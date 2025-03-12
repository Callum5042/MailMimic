namespace MailMimic.TestWeb.Models;

#nullable disable

public class MailboxModel
{
    public IEnumerable<ListItem> Emails { get; set; } = [];

    public class ListItem
    {
        public Guid Id { get; set; }

        public string MailFrom { get; set; }

        public string Subject { get; set; }

        public string Size { get; set; }

        public string DateTime { get; set; }
    }
}
