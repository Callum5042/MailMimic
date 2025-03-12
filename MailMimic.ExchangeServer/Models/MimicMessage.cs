namespace MailMimic.ExchangeServer.Models;

public class MimicMessage
{
    public MimicMessage()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; private set; }

    public IList<string> MailFrom { get; set; } = [];

    public IList<string> MailTo { get; set; } = [];

    public string? Source { get; set; }

    public string Subject 
    {
        get
        {
            return "Test Subject";
        }
    }

    public DateTime DateTime { get; set; } = DateTime.UtcNow;
}
