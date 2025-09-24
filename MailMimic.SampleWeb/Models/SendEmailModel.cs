using System.ComponentModel.DataAnnotations;

namespace MailMimic.SampleWeb.Models;

#nullable disable

public class SendEmailModel
{
    [Display(Name = "Mail To")]
    [Required]
    public string MailTo { get; set; }

    [Display(Name = "Mail From")]
    [Required]
    public string MailFrom { get; set; }

    [Required]
    public string Subject { get; set; }

    public string Body { get; set; }
}
