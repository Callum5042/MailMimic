using MailKit.Net.Smtp;
using MailMimic.MailStores;
using MailMimic.SampleWeb.Models;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace MailMimic.SampleWeb.Controllers;

public class HomeController : Controller
{
    public IActionResult Index(bool emailSent, [FromServices] IMimicStore mimicStore)
    {
        ViewBag.EmailSent = emailSent;

        var model = new SendEmailModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SendEmail([FromForm] SendEmailModel model)
    {
        // Build the email
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(model.MailFrom, model.MailFrom));
        message.To.Add(new MailboxAddress(model.MailTo, model.MailTo));
        message.Subject = model.Subject;

        message.Body = new TextPart("plain")
        {
            Text = model.Body,
        };

        // Send the email
        using var client = new SmtpClient();
        await client.ConnectAsync("localhost", 465, useSsl: true);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        // Redirect back to the index page
        return RedirectToAction(nameof(Index), new 
        { 
            emailSent = true 
        });
    }

    [HttpPost]
    public async Task<IActionResult> QuickSendEmail()
    {
        // Build the email
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("RoveTech", "from@rovetech.com"));
        message.To.Add(new MailboxAddress("Neo", "neo@matrix.com"));
        message.To.Add(new MailboxAddress("Morpheus", "morpheus@matrix.com"));
        message.Cc.Add(new MailboxAddress("The Architect", "architect@matrix.com"));
        message.Subject = "Quick Send Email Test";

        //message.Body = new TextPart("plain")
        //{
        //    Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque laoreet rutrum suscipit. Nulla ut turpis nec est finibus posuere ac a velit. Donec aliquet ex sed turpis imperdiet sollicitudin. In euismod, nisl vitae aliquet gravida, eros ex tempus quam, eget congue erat quam vel ex. Suspendisse nec felis nec ligula commodo vehicula. Mauris facilisis augue sem, non cursus metus laoreet ac. Sed sit amet purus scelerisque, efficitur turpis eget, efficitur quam. Vestibulum aliquet orci nec leo iaculis ultrices. Sed in felis congue, scelerisque diam vitae, consectetur arcu. Cras vel enim at velit placerat porttitor. Vestibulum id neque urna. Donec auctor aliquam pellentesque. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos.",
        //};

        var bodyBuilder = new BodyBuilder();
        // bodyBuilder.HtmlBody = "<b>This is some html text</b>";
        bodyBuilder.TextBody = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque laoreet rutrum suscipit. Nulla ut turpis nec est finibus posuere ac a velit. Donec aliquet ex sed turpis imperdiet sollicitudin. In euismod, nisl vitae aliquet gravida, eros ex tempus quam, eget congue erat quam vel ex. Suspendisse nec felis nec ligula commodo vehicula. Mauris facilisis augue sem, non cursus metus laoreet ac. Sed sit amet purus scelerisque, efficitur turpis eget, efficitur quam. Vestibulum aliquet orci nec leo iaculis ultrices. Sed in felis congue, scelerisque diam vitae, consectetur arcu. Cras vel enim at velit placerat porttitor. Vestibulum id neque urna. Donec auctor aliquam pellentesque. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos.";

        bodyBuilder.Attachments.Add(@"C:\Temp\test.txt");
        bodyBuilder.Attachments.Add(@"C:\Temp\test2.zip");

        message.Body = bodyBuilder.ToMessageBody();

        // Send the email
        using var client = new SmtpClient();
        await client.ConnectAsync("localhost", 465, useSsl: true);
        // await client.AuthenticateAsync("Admin", "Password");
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        // Redirect back to the index page
        return RedirectToAction(nameof(Index), new 
        { 
            emailSent = true 
        });
    }
}
