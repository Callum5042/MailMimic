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

        var ipAddressType = _options.Value.ListenOnAllInterfaces ? IPAddress.Any : IPAddress.Loopback;
        var tcpServer = new TcpListener(ipAddressType, _options.Value.Port);
        tcpServer.Start();

        _logger.LogInformation("SMTP server is running");

        while (!stoppingToken.IsCancellationRequested)
        {
            var client = await tcpServer.AcceptTcpClientAsync(stoppingToken);

            _logger.LogInformation("Client connected");

            // Handle the SMTP session. Not passing the token to Task.Run: if it were cancelled before the
            // delegate starts, the delegate would never run and the client would never be disposed.
            _ = Task.Run(() => HandleSmtpSession(client, stoppingToken), CancellationToken.None);
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
            try
            {
                await sslStream.AuthenticateAsServerAsync(serverCertificate, clientCertificateRequired: false, SslProtocols.Tls12, true);
            }
            catch
            {
                await sslStream.DisposeAsync();
                throw;
            }

            return sslStream;
        }

        return client.GetStream();
    }

    private async Task HandleSmtpSession(TcpClient client, CancellationToken cancellationToken)
    {
        using (client)
        {
            try
            {
                await using var stream = await GetClientStreamAsync(client);
                await _smtpSession.HandleAsync(stream, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Server is shutting down
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMTP session failed");
            }
            finally
            {
                _logger.LogInformation("Client disconnected");
            }
        }
    }
}
