namespace MailMimic.Services;

public interface ISmtpParser
{
    SmtpData Parse(string source);
}