using MailMimic.MailStores;
using MailMimic.Portal.Models;
using MailMimic.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace MailMimic.Portal.Controllers;

[Area("MailMimic")]
public class MailboxController : Controller
{
    private readonly IMimicStore _mimicStore;
    private readonly ISmtpParser _smtpParser;

    public MailboxController(IMimicStore mimicStore, ISmtpParser smtpParser)
    {
        _mimicStore = mimicStore;
        _smtpParser = smtpParser;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var mailboxes = await _mimicStore.GetAllAsync();

        if (!string.IsNullOrEmpty(search))
        {
            mailboxes = mailboxes
                .Where(x => x.Source?.Contains(search, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
        }

        var model = new MailboxModel
        {
            Emails = mailboxes.Select(x => new MailboxModel.ListItem
            {
                Id = x.Id,
                MailFrom = x.MailFrom.FirstOrDefault(),
                Subject = new SmtpParser().Parse(x.Source!).Subject,
                Size = $"{Encoding.Unicode.GetByteCount(x.Source ?? string.Empty)} bytes",
                DateTime = x.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            })
        };

        ViewBag.Search = search;
        return View("~/Views/Mailbox/Index.cshtml", model);
    }

    [HttpPost]
    public IActionResult Search(string? search)
    {
        return RedirectToAction(nameof(Index), new { search });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAll()
    {
        await _mimicStore.DeleteAll();
        return RedirectToAction(nameof(Index));
    }

    [Route("[area]/[controller]/{id:guid}")]
    public async Task<IActionResult> Email(Guid id)
    {
        var model = await _mimicStore.FindAsync(id);
        if (model == null)
        {
            return NotFound();
        }

        var smtpData = _smtpParser.Parse(model!.Source!);
        return View("~/Views/Mailbox/Email.cshtml", smtpData);
    }
}