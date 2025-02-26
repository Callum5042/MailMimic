using MailKit.Net.Smtp;
using MimeKit;

namespace MailMimic;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        Console.ReadLine();
        Console.WriteLine("Sent email");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Joey Tribbiani", "joey@friends.com"));
        message.To.Add(new MailboxAddress("Mrs. Chanandler Bong", "chandler@friends.com"));
        message.To.Add(new MailboxAddress("Mrs. Test Human", "test@email.com"));
        message.Subject = "How you doin'?";

        message.Body = new TextPart("plain")
        {
            Text = """  
    Hey Chandler,  

    Monica and I were going to play some paintball, you in?  

    -- Joey  
    """
        };

        using var client = new SmtpClient();
        client.Connect("localhost", 587, false);
        // Note: only needed if the SMTP server requires authentication  
        // client.Authenticate("joey", "password");
        client.Send(message);
        client.Disconnect(true);
    }
}
