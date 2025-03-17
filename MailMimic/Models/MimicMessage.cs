using System.Text;

namespace MailMimic.Models;

public class MimicMessage
{
    public MimicMessage()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }

    public IList<string> MailFrom { get; set; } = [];

    public IList<string> MailTo { get; set; } = [];

    public string? Source { get; set; }

    public string? Subject { get; set; }

    public string? Body { get; set; }

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
                    var parts = line.Split([':'], 2);

                    Headers.Add(parts[0].Trim(), parts[1].Trim());
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
        Source = source;

        if (Headers.TryGetValue("Subject", out string? value))
        {
            Subject = value;
        }
    }
}
