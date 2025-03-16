﻿using MailMimic.MailStores;
using MailMimic.Models;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.Extensions.Logging;

namespace MailMimic.Services;

public class SmtpSession : ISmtpSession
{
    private readonly ILogger<SmtpSession> _logger;
    private readonly IMimicStore _mimicStore;

    public SmtpSession(ILogger<SmtpSession> logger, IMimicStore mimicStore)
    {
        _logger = logger;
        _mimicStore = mimicStore;
    }

    public async Task HandleAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream, Encoding.ASCII, leaveOpen: true);
        using var writer = new StreamWriter(stream, Encoding.ASCII, leaveOpen: true) { AutoFlush = true };

        var mimicMessage = new MimicMessage();

        // RFC 5321, Section 3 - The SMTP Procedures: An Overview
        // RFC 5321, Section 3.1 - Session Initiation
        await writer.WriteLineAsync("220 MailMimic SMTP ready");


        var line = await reader.ReadLineAsync(cancellationToken);
        while (!string.IsNullOrEmpty(line))
        {
            _logger.LogInformation("Received: " + line);

            // Respond to specific SMTP commands
            if (line.StartsWith("EHLO", StringComparison.OrdinalIgnoreCase) || line.StartsWith("HELO", StringComparison.OrdinalIgnoreCase))
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
    }
}
