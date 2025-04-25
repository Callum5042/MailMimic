using MailMimic.Portal.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace MailMimic.Portal.Extensions;

/// <summary>
/// Extension methods for adding the MailMimic portal to the web application.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Adds the MailMimic portal to the web application.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <param name="routePrefix">Route prefix used for the route pattern.</param>
    /// <returns>The web application.</returns>
    public static WebApplication MapMailMimicPortal(this WebApplication app, string routePrefix = "MailMimic")
    {
        // Map the area route for the MailMimic portal
        app.MapAreaControllerRoute(
            name: "mailmimic_route",
            areaName: "MailMimic",
            pattern: routePrefix + "/{controller}/{action=Index}/{id?}");

        // Map the wwwroot folder in the assembly to the /mailmimic-static path
        var embeddedProvider = new ManifestEmbeddedFileProvider(typeof(MailboxController).Assembly, "wwwroot");
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = embeddedProvider,
            RequestPath = "/mailmimic-static"
        });

        return app;
    }
}
