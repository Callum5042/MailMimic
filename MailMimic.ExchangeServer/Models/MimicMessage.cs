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

    public string? Body { get; set; }
}
