namespace MailMimic.Portal.Models;

#nullable disable

internal class MailboxModel
{
    public IEnumerable<ListItem> Emails { get; set; } = new List<ListItem>();

    public class ListItem
    {
        public Guid Id { get; set; }

        public string MailFrom { get; set; }

        public string Subject { get; set; }

        public string Size { get; set; }

        public string DateTime { get; set; }
    }
}