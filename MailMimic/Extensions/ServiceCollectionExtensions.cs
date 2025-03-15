using MailMimic.MailStores;
using Microsoft.Extensions.DependencyInjection;

namespace MailMimic.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMailMimic(this IServiceCollection services)
    {
        services.AddHostedService<MailExchangeService>();

        services.AddSingleton<IMimicStore, InMemoryMimicStore>();

        return services;
    }
}
