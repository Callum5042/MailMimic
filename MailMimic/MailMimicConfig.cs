namespace MailMimic;

#nullable disable

public class MailMimicConfig
{
    public ushort Port { get; set; } = 465;

    public bool UseSsl { get; set; } = true;

    public string SslThumbprint { get; set; }
}
