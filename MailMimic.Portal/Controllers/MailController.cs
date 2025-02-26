using Microsoft.AspNetCore.Mvc;

namespace MailMimic.Portal.Controllers;

[Area("MailMimic")]
public class MailController : Controller
{
    // GET /MailMimic/Mail/Index
    [HttpGet]
    public IActionResult Index()
    {
        return View("~/Views/Test/Index.cshtml");
    }
}