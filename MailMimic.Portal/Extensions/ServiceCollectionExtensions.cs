using MailMimic.Portal.Controllers;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace MailMimic.Portal.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMailMimicPortal(this IServiceCollection services)
    {
        // services.AddMailMimic();

        services.AddControllersWithViews()
            .AddApplicationPart(typeof(MailController).Assembly);

        services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
        {
            options.FileProviders.Add(new EmbeddedFileProvider(typeof(MailController).Assembly));
        });

        return services;
    }
}
