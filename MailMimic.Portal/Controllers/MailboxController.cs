using MailMimic.ExchangeServer.MailStores;
using MailMimic.Portal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace MailMimic.Portal.Controllers;

[Area("MailMimic")]
public class MailboxController : Controller
{
    private readonly IMimicStore _mimicStore;

    public MailboxController(IMimicStore mimicStore)
    {
        _mimicStore = mimicStore;
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
                Subject = x.Subject,
                Size = $"{Encoding.Unicode.GetByteCount(x.Source!)} bytes",
                DateTime = x.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            })
        };

        return View("~/Views/Mailbox/Index.cshtml", model);
    }

    [Route("[area]/[controller]/{id:guid}")]
    public async Task<IActionResult> Email(Guid id)
    {
        var model = await _mimicStore.FindAsync(id);

        return View("~/Views/Mailbox/Email.cshtml", model);
    }
}
