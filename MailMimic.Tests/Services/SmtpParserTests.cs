using MailMimic.Services;

namespace MailMimic.Tests.Services;

public class SmtpParserTests
{
    [Fact]
    public void Parse_BasicText()
    {
        // Arrange
        var source = """
            From: RoveTech <from@rovetech.com>
            Date: Mon, 24 Mar 2025 17:04:51 +0000
            Subject: Quick Send Email Test
            Message-Id: <V1113M85QPU4.T3YXZXLWOREP3@desktop-0cd7b50>
            To: Neo <neo@matrix.com>, Morpheus <morpheus@matrix.com>
            Cc: The Architect <architect@matrix.com>
            MIME-Version: 1.0
            Content-Type: text/plain; charset=utf-8

            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque laoreet rutrum suscipit. Nulla ut turpis nec est finibus posuere ac a velit. Donec aliquet ex sed turpis imperdiet sollicitudin. In euismod, nisl vitae aliquet gravida, eros ex tempus quam, eget congue erat quam vel ex. Suspendisse nec felis nec ligula commodo vehicula. Mauris facilisis augue sem, non cursus metus laoreet ac. Sed sit amet purus scelerisque, efficitur turpis eget, efficitur quam. Vestibulum aliquet orci nec leo iaculis ultrices. Sed in felis congue, scelerisque diam vitae, consectetur arcu. Cras vel enim at velit placerat porttitor. Vestibulum id neque urna. Donec auctor aliquam pellentesque. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos.
            """;

        // Act
        var smtpParser = new SmtpParser();
        var result = smtpParser.Parse(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(8, result.Headers.Count);
        Assert.Equal("Quick Send Email Test", result.Subject);
        Assert.Equal("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque laoreet rutrum suscipit. Nulla ut turpis nec est finibus posuere ac a velit. Donec aliquet ex sed turpis imperdiet sollicitudin. In euismod, nisl vitae aliquet gravida, eros ex tempus quam, eget congue erat quam vel ex. Suspendisse nec felis nec ligula commodo vehicula. Mauris facilisis augue sem, non cursus metus laoreet ac. Sed sit amet purus scelerisque, efficitur turpis eget, efficitur quam. Vestibulum aliquet orci nec leo iaculis ultrices. Sed in felis congue, scelerisque diam vitae, consectetur arcu. Cras vel enim at velit placerat porttitor. Vestibulum id neque urna. Donec auctor aliquam pellentesque. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos.", result.Contents.First().Body);
    }
    
    [Fact]
    public void Parse_Complex()
    {
        // Arrange
        var source = """
            From: RoveTech <from@rovetech.com>
            Date: Sun, 23 Mar 2025 21:21:02 +0000
            Subject: Quick Send Email Test
            Message-Id: <MC8S4B6WPPU4.BNYTX8R5LZPB2@desktop-0cd7b50>
            To: Neo <neo@matrix.com>, Morpheus <morpheus@matrix.com>
            Cc: The Architect <architect@matrix.com>
            MIME-Version: 1.0
            Content-Type: multipart/mixed; boundary="=-Pr2P063p1b6Rxl3w77+NLQ=="

            --=-Pr2P063p1b6Rxl3w77+NLQ==
            Content-Type: text/plain; charset=utf-8

            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque laoreet rutrum suscipit. Nulla ut turpis nec est finibus posuere ac a velit. Donec aliquet ex sed turpis imperdiet sollicitudin. In euismod, nisl vitae aliquet gravida, eros ex tempus quam, eget congue erat quam vel ex. Suspendisse nec felis nec ligula commodo vehicula. Mauris facilisis augue sem, non cursus metus laoreet ac. Sed sit amet purus scelerisque, efficitur turpis eget, efficitur quam. Vestibulum aliquet orci nec leo iaculis ultrices. Sed in felis congue, scelerisque diam vitae, consectetur arcu. Cras vel enim at velit placerat porttitor. Vestibulum id neque urna. Donec auctor aliquam pellentesque. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos.
            --=-Pr2P063p1b6Rxl3w77+NLQ==
            Content-Type: text/plain; name=test.txt
            Content-Transfer-Encoding: 7bit
            Content-Disposition: attachment; filename=test.txt
               
            testing
            --=-Pr2P063p1b6Rxl3w77+NLQ==
            Content-Type: application/zip; name=test2.zip
            Content-Transfer-Encoding: base64
            Content-Disposition: attachment; filename=test2.zip

            UEsDBBQAAAAAAM+pd1rwCSXoCgAAAAoAAAAJAAAAdGVzdDIudHh0dGVzdGluZzEyM1BLAQIUABQA
            AAAAAM+pd1rwCSXoCgAAAAoAAAAJAAAAAAAAAAEAIAAAAAAAAAB0ZXN0Mi50eHRQSwUGAAAAAAEA
            AQA3AAAAMQAAAAAA

            --=-Pr2P063p1b6Rxl3w77+NLQ==--
            """;

        // Act
        var smtpParser = new SmtpParser();
        var result = smtpParser.Parse(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Contents.Count);
    }
}
