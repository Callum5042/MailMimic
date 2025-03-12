using MailMimic.ExchangeServer.MailStores;
using Microsoft.AspNetCore.Mvc;

namespace MailMimic.Portal.Controllers;

[Area("MailMimic")]
public class MailController : Controller
{
    private readonly IMimicStore _mimicStore;

    public MailController(IMimicStore mimicStore)
    {
        _mimicStore = mimicStore;
    }

    // GET /MailMimic/Mail/Index
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var result = await _mimicStore.GetAllAsync();
        return View("~/Views/Test/Index.cshtml", result);
    }
}