using MailMimic.MailStores;
using MailMimic.Models;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace MailMimic;

public class MailExchangeService : BackgroundService
{
    private readonly IMimicStore _mimicStore;

    public MailExchangeService(IMimicStore mimicStore)
    {
        _mimicStore = mimicStore;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tcpServer = new TcpListener(IPAddress.Loopback, 587);
        tcpServer.Start();
        Console.WriteLine("SMTP server is running...");

        while (!stoppingToken.IsCancellationRequested)
        {
            var client = await tcpServer.AcceptTcpClientAsync(stoppingToken);
            Console.WriteLine("Client connected.");

            // Handle the SMTP session
            _ = Task.Run(() => HandleSmtpSession(client, stoppingToken));
        }
    }

    private async Task HandleSmtpSession(TcpClient client, CancellationToken cancellationToken)
    {
        // RFC 5321, Section 3 - The SMTP Procedures: An Overview

        using var networkStream = client.GetStream();
        using var reader = new StreamReader(networkStream, Encoding.ASCII);
        using var writer = new StreamWriter(networkStream, Encoding.ASCII) { AutoFlush = true };

        // RFC 5321, Section 3.1 - Session Initiation
        await writer.WriteLineAsync("220 MailMimic SMTP ready");

        var mimicMessage = new MimicMessage();

        var line = await reader.ReadLineAsync(cancellationToken);
        while (!string.IsNullOrEmpty(line))
        {
            Console.WriteLine($"Received: {line}");

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

                Console.WriteLine("Message received:");
                Console.WriteLine(messageBuilder.ToString());

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

        Console.WriteLine("Client disconnected.");
        client.Dispose();
    }
}
