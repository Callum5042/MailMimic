namespace MailMimic;

#nullable disable

/// <summary>
/// Configuration settings for MailMimic.
/// </summary>
public class MailMimicConfig
{
    /// <summary>
    /// The SMTP port used for communication.
    /// Default: 465 (commonly used for implicit SSL/TLS).
    /// </summary>
    public ushort Port { get; set; } = 465;

    /// <summary>
    /// Determines whether SSL is required for connections.
    /// Default: true.
    /// </summary>
    public bool UseSsl { get; set; } = true;

    /// <summary>
    /// The SSL certificate thumbprint to use for authentication (if applicable).
    /// Required only if a specific certificate needs to be enforced.
    /// </summary>
    public string SslThumbprint { get; set; }

    /// <summary>
    /// The username for authentication
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The password for authentication
    /// </summary>
    public string Password { get; set; }
}
