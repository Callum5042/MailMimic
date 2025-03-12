using MailMimic.ExchangeServer.MailStores;
using MailMimic.TestWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace MailMimic.TestWeb.Controllers;

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

        return View(model);
    }

    [Route("[controller]/{id:guid}")]
    public async Task<IActionResult> Email(Guid id)
    {
        var model = await _mimicStore.FindAsync(id);

        return View(model);
    }
}
