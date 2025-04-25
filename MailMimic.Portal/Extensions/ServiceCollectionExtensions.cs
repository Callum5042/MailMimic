using MailMimic.Portal.Controllers;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace MailMimic.Portal.Extensions;

/// <summary>
/// Extension methods for adding the MailMimic portal to the service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the MailMimic portal to the service collection.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddMailMimicPortal(this IServiceCollection services)
    {
        services.AddControllersWithViews()
            .AddApplicationPart(typeof(MailboxController).Assembly);

        services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
        {
            options.FileProviders.Add(new EmbeddedFileProvider(typeof(MailboxController).Assembly));
        });

        return services;
    }
}
