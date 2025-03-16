using MailMimic.MailStores;
using MailMimic.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace MailMimic;

public class MailExchangeService : BackgroundService
{
    private readonly IMimicStore _mimicStore;
    private readonly ILogger<MailExchangeService> _logger;
    private readonly IOptions<MailMimicConfig> _options;

    public MailExchangeService(IMimicStore mimicStore, ILogger<MailExchangeService> logger, IOptions<MailMimicConfig> options)
    {
        _mimicStore = mimicStore;
        _logger = logger;
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _logger.BeginScope("{Service}", nameof(MailExchangeService));
        _logger.LogInformation("starting SMTP server");

        var tcpServer = new TcpListener(IPAddress.Loopback, _options.Value.Port);
        tcpServer.Start();

        _logger.LogInformation("SMTP server is running");

        while (!stoppingToken.IsCancellationRequested)
        {
            var client = await tcpServer.AcceptTcpClientAsync(stoppingToken);
            _logger.LogInformation("Client connected");

            // Handle the SMTP session
            _ = Task.Run(() => HandleSmtpSession(client, stoppingToken));
        }
    }

    private X509Certificate2 LoadCertificate(string thumbprint)
    {
        using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);

        var certs = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, validOnly: true);
        if (!certs.Any())
        {
            throw new Exception("Unable to find X509 certificate");
        }

        return certs[0];
    }

    private static X509Certificate2 GetDevelopmentCertificate()
    {
        using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);

        var certs = store.Certificates.Find(X509FindType.FindBySubjectName, "localhost", validOnly: false);
        if (!certs.Any())
        {
            throw new Exception("Unable to find X509 certificate");
        }

        return certs[0];
    }

    private async Task HandleSmtpSession(TcpClient client, CancellationToken cancellationToken)
    {
        // RFC 5321, Section 3 - The SMTP Procedures: An Overview
        using var networkStream = client.GetStream();

        var useSsl = _options.Value.UseSsl;
        using var sslStream = new SslStream(client.GetStream(), false);
        if (useSsl)
        {
            X509Certificate2? serverCertificate = null;
            if (string.IsNullOrEmpty(_options.Value.SslThumbprint))
            {
                serverCertificate = GetDevelopmentCertificate();
            }
            else
            {
                serverCertificate = LoadCertificate(_options.Value.SslThumbprint);
            }

            await sslStream.AuthenticateAsServerAsync(serverCertificate, clientCertificateRequired: false, SslProtocols.Tls12, true);
        }

        using var reader = new StreamReader(useSsl ? sslStream : networkStream, Encoding.ASCII);
        using var writer = new StreamWriter(useSsl ? sslStream : networkStream, Encoding.ASCII) { AutoFlush = true };


        // RFC 5321, Section 3.1 - Session Initiation
        await writer.WriteLineAsync("220 MailMimic SMTP ready");

        var mimicMessage = new MimicMessage();

        var line = await reader.ReadLineAsync(cancellationToken);
        while (!string.IsNullOrEmpty(line))
        {
            _logger.LogInformation("Received: " + line);

            // Respond to specific SMTP commands
            if (line.StartsWith("EHLO", StringComparison.OrdinalIgnoreCase))
            {
                // RFC 5321, Section 3.2 - Client Initiation
                // RFC 5321, Section 4.1.1.1 - Extended HELLO (EHLO) or HELLO (HELO)

                const string domain = "localhost";
                await writer.WriteLineAsync($"250-Hello {domain} MailMimic");
                await writer.WriteLineAsync("250 OK");
            }
            else if (line.StartsWith("MAIL FROM:", StringComparison.OrdinalIgnoreCase))
            {
                var regex = Regex.Match(line, @"^MAIL FROM:<(.*)>$", RegexOptions.Compiled);
                mimicMessage.MailFrom.Add(regex.Groups[1].Value);

                // RFC 5321, Section 3.3 - Mail Transactions
                await writer.WriteLineAsync("250 OK");
            }
            else if (line.StartsWith("RCPT TO:", StringComparison.OrdinalIgnoreCase))
            {
                var regex = Regex.Match(line, @"^RCPT TO:<(.*)>$", RegexOptions.Compiled);
                mimicMessage.MailTo.Add(regex.Groups[1].Value);

                // RFC 5321, Section 3.3 - Mail Transactions
                await writer.WriteLineAsync("250 OK");
            }
            else if (line.StartsWith("DATA", StringComparison.OrdinalIgnoreCase))
            {
                // RFC 5321, Section 3.3 - Mail Transactions
                await writer.WriteLineAsync("354 Start mail input; end with <CRLF>.<CRLF>");

                // Read message content until a single dot (".") on a line
                var messageBuilder = new StringBuilder();
                while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
                {
                    if (line == ".")
                        break;

                    messageBuilder.AppendLine(line);
                }

                //Console.WriteLine("Message received:");
                //Console.WriteLine(messageBuilder.ToString());

                mimicMessage.SetSource(messageBuilder.ToString());
                await _mimicStore.AddAsync(mimicMessage);

                await writer.WriteLineAsync("250 Message accepted");
            }
            else if (line.StartsWith("QUIT", StringComparison.OrdinalIgnoreCase))
            {
                await writer.WriteLineAsync("221 Bye");
                break;
            }
            else
            {
                await writer.WriteLineAsync("500 Command not recognized");
            }

            // Try get next line
            line = await reader.ReadLineAsync(cancellationToken);
        }

        _logger.LogInformation("Client disconnected");
        client.Dispose();
    }
}
