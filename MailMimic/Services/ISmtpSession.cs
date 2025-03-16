namespace MailMimic.Services;

public interface ISmtpSession
{
    Task HandleAsync(Stream stream, CancellationToken cancellationToken);
}
