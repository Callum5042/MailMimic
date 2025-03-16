using MailMimic.MailStores;
using MailMimic.Services;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text;

namespace MailMimic.Tests.Services;

public class SmtpSessionTests
{
    [Fact]
    public async Task HandleAsync_SmtpReady_WritesToStream()
    {
        // Arrange
        var logger = new NullLogger<SmtpSession>();
        var mimicStore = Substitute.For<IMimicStore>();

        using var stream = new MemoryStream();
        using var reader = new StreamReader(stream, Encoding.ASCII);

        // Act
        var smtpSession = new SmtpSession(logger, mimicStore, null!);
        await smtpSession.HandleAsync(stream, CancellationToken.None);

        // Assert
        stream.Position = 0;
        var line = await reader.ReadToEndAsync();

        Assert.Contains("220 MailMimic SMTP ready", line);
    }
}
