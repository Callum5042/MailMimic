using MailMimic.ExchangeServer.MailStores;
using Microsoft.Extensions.DependencyInjection;

namespace MailMimic.ExchangeServer.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddMailMimic(this IServiceCollection services)
    {
        services.AddHostedService<MailExchangeService>();

        services.AddSingleton<IMimicStore, InMemoryMimicStore>();
    }
}
