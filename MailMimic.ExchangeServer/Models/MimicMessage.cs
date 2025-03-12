﻿using System.Text;

namespace MailMimic.ExchangeServer.Models;

public class MimicMessage
{
    public MimicMessage()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; private set; }

    public IList<string> MailFrom { get; set; } = [];

    public IList<string> MailTo { get; set; } = [];

    public string? Source { get; private set; }

    public string? Subject { get; private set; }

    public string? Body { get; private set; }

    public DateTime DateTime { get; set; } = DateTime.UtcNow;

    public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

    public void SetSource(string source)
    {
        bool parseHeaders = true;

        var lines = source.Split(Environment.NewLine);
        var body = new StringBuilder();

        foreach (var line in lines)
        {
            // Parse headers
            if (parseHeaders)
            {
                if (line.Contains(":"))
                {
                    var headerLine = line.Split(":");
                    Headers.Add(headerLine[0].Trim(), headerLine[1].Trim());
                }
                else
                {
                    // No more headers found so we move onto the body
                    parseHeaders = false;
                    continue;
                }
            }
            else
            {
                body.AppendLine(line);
            }
        }

        // Body
        Body = body.ToString();
    }
}
