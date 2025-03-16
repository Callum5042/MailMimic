using MailMimic.MailStores;
using MailMimic.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MailMimic.Extensions;

/// <summary>
/// Extension classes for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers MailMimic for dependency injection and binds its configuration 
    /// from the "MailMimicConfig" section in appsettings.json.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddMailMimic(this IServiceCollection services)
    {
        services.AddMailMimicCore();
        services.AddOptions<MailMimicConfig>().BindConfiguration(nameof(MailMimicConfig));

        return services;
    }

    /// <summary>
    /// Registers MailMimic for dependency injection with custom configuration.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="config">The MailMimic configuration options <see cref="MailMimicConfig"/></param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddMailMimic(this IServiceCollection services, Action<MailMimicConfig> config)
    {
        services.AddMailMimicCore();
        services.AddOptions<MailMimicConfig>().Configure(config);

        return services;
    }

    /// <summary>
    /// Registers core MailMimic services.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <returns>The updated service collection.</returns>
    internal static IServiceCollection AddMailMimicCore(this IServiceCollection services)
    {
        services.AddHostedService<MailExchangeService>();
        services.AddSingleton<IMimicStore, InMemoryMimicStore>();
        services.AddTransient<ICertificateLoader, CertificateLoader>();
        services.AddTransient<ISmtpSession, SmtpSession>();

        return services;
    }
}
