using System.ComponentModel.DataAnnotations;

namespace MailMimic.TestWeb.Models;

#nullable disable

public class SendEmailModel
{
    [Display(Name = "Mail To")]
    public string MailTo { get; set; }

    [Display(Name = "Mail From")]
    public string MailFrom { get; set; }

    public string Subject { get; set; }

    public string Body { get; set; }
}
