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

    public async Task<IActionResult> Index()
    {
        var mailboxes = await _mimicStore.GetAllAsync();

        var model = new MailboxModel
        {
            Emails = mailboxes.Select(x => new MailboxModel.ListItem
            {
                Id = x.Id,
                MailFrom = x.MailFrom.FirstOrDefault(),
                Subject = "Boo",
                Size = $"{Encoding.Unicode.GetByteCount(x.Source ?? string.Empty)} bytes",
                DateTime = x.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            })
        };

        return View("~/Views/Mailbox/Index.cshtml", model);
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

//public enum EmailView
//{
//    Content,
//    Source,
//    Attachments,
//}