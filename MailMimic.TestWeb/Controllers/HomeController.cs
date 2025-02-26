using MailKit.Net.Smtp;
using MailMimic.TestWeb.Models;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace MailMimic.TestWeb.Controllers;

public class HomeController : Controller
{
    public IActionResult Index(bool emailSent)
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
        await client.ConnectAsync("localhost", 587, false);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        // Redirect back to the index page
        return RedirectToAction(nameof(Index), new 
        { 
            emailSent = true 
        });
    }
}
