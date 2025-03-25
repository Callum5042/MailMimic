namespace MailMimic.Models;

/// <summary>
/// Represents a MIME message.
/// </summary>
public class MimicMessageEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MimicMessageEntity"/> class with the <see cref="Id"/> generated.
    /// </summary>
    public MimicMessageEntity()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// The unique identifier of the message.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The sender of the message read from the SMTP MAIL FROM command.
    /// </summary>
    public IList<string> MailFrom { get; set; } = [];

    /// <summary>
    /// The recipients of the message read from the SMTP RCPT TO command.
    /// </summary>
    public IList<string> MailTo { get; set; } = [];

    /// <summary>
    /// The source of the message read from the SMTP DATA command.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// The date and time the message was received.
    /// </summary>
    public DateTime DateTime { get; set; } = DateTime.UtcNow;
}
