using MailMimic.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace MailMimic;

public class MailExchangeService : BackgroundService
{
    private readonly ILogger<MailExchangeService> _logger;
    private readonly IOptions<MailMimicConfig> _options;
    private readonly ICertificateLoader _certificateLoader;
    private readonly ISmtpSession _smtpSession;

    public MailExchangeService(ILogger<MailExchangeService> logger, IOptions<MailMimicConfig> options, ICertificateLoader certificateLoader, ISmtpSession smtpSession)
    {
        _logger = logger;
        _options = options;
        _certificateLoader = certificateLoader;
        _smtpSession = smtpSession;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _logger.BeginScope("{Service}", nameof(MailExchangeService));
        _logger.LogInformation("starting SMTP server");

        var tcpServer = new TcpListener(IPAddress.Any, _options.Value.Port);
        tcpServer.Start();

        _logger.LogInformation("SMTP server is running");

        while (!stoppingToken.IsCancellationRequested)
        {
            var client = await tcpServer.AcceptTcpClientAsync(stoppingToken);

            _logger.LogInformation("Client connected");

            // Handle the SMTP session
            _ = Task.Run(() => HandleSmtpSession(client, stoppingToken), stoppingToken);
        }
    }

    private X509Certificate2 GetX509Certificate()
    {
        if (string.IsNullOrEmpty(_options.Value.SslThumbprint))
        {
            return _certificateLoader.GetDevelopmentCertificate();
        }
        else
        {
            return _certificateLoader.LoadCertificate(_options.Value.SslThumbprint);
        }
    }

    private async Task<Stream> GetClientStreamAsync(TcpClient client)
    {
        var useSsl = _options.Value.UseSsl;
        if (useSsl)
        {
            var serverCertificate = GetX509Certificate();

            var sslStream = new SslStream(client.GetStream(), false);
            await sslStream.AuthenticateAsServerAsync(serverCertificate, clientCertificateRequired: false, SslProtocols.Tls12, true);

            return sslStream;
        }

        return client.GetStream();
    }

    private async Task HandleSmtpSession(TcpClient client, CancellationToken cancellationToken)
    {
        using var stream = await GetClientStreamAsync(client);
        await _smtpSession.HandleAsync(stream, cancellationToken);

        _logger.LogInformation("Client disconnected");
        client.Dispose();
    }
}
