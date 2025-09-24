using MailMimic.Extensions;
using MailMimic.Portal.Extensions;

namespace MailMimic.DockerHost;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddMailMimic();
        builder.Services.AddMailMimicPortal();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        app.MapMailMimicPortal();

        app.Run();
    }
}
