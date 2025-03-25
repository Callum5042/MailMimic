using MailMimic.Extensions;
using System.Text.RegularExpressions;

namespace MailMimic.Services;

public class SmtpData
{
    public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

    public ICollection<SmtpContent> Contents { get; set; } = [];

    public string Subject => Headers["Subject"] ?? string.Empty;
}

public class SmtpContent
{
    public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

    public string? Body { get; set; }
}

public class SmtpParser : ISmtpParser
{
    public SmtpData Parse(string source)
    {
        ParsingHeader = true;

        var data = new SmtpData();

        using var reader = new StringReader(source);

        string? line = null;
        while ((line = reader.ReadLine()) != null)
        {
            if (ParsingHeader)
            {
                // Start with reading the headers
                if (line == string.Empty)
                {
                    ParsingHeader = false;
                }
                else
                {
                    var (key, value) = line.SplitKeyValue();
                    data.Headers.TryAdd(key, value);
                }
            }
            else
            {
                // Finished headers now parse body depending on content type
                if (data.Headers.TryGetValue("Content-Type", out var contentType))
                {
                    if (contentType.StartsWith("multipart/mixed"))
                    {
                        // Parse multipart content
                        var match = Regex.Match(contentType, "boundary=\"(.+)\"", RegexOptions.Compiled);
                        var boundary = match.Groups[1].Value;

                        SmtpContent? content = null;

                        do
                        {
                            if (line.Contains(boundary))
                            {
                                if (content != null)
                                {
                                    data.Contents.Add(content);
                                }

                                content = new SmtpContent();
                                ParsingContentHeader = true;
                            }
                            else
                            {
                                if (line == string.Empty)
                                {
                                    ParsingContentHeader = false;
                                }
                                else
                                {
                                    if (ParsingContentHeader)
                                    {
                                        var (key, value) = line.SplitKeyValue();
                                        content!.Headers.TryAdd(key, value);
                                    }
                                    else
                                    {
                                        content!.Body += line;
                                    }
                                }
                            }
                        }
                        while ((line = reader.ReadLine()) != null);
                    }
                    else
                    {
                        // Parse single content
                        var content = new SmtpContent
                        {
                            Body = line,
                        };

                        content.Headers.TryAdd("Content-Type", contentType);
                        data.Contents.Add(content);
                    }
                }
            }
        }

        return data;
    }

    private bool ParsingHeader { get; set; }

    private bool ParsingContentHeader { get; set; }
}
