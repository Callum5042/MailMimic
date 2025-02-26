using MailMimic.ExchangeServer.Extensions;
using Microsoft.Extensions.Hosting;

namespace MailMimic.ExchangeServer;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMailMimic();
            });

        var host = builder.Build();
        await host.RunAsync();
    }
}
