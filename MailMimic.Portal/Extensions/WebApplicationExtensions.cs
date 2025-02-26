using Microsoft.AspNetCore.Builder;

namespace MailMimic.Portal.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MapMailMimicPortal(this WebApplication app, string routePrefix = "MailMimic")
    {
        app.MapAreaControllerRoute(
            name: "mailmimic_route",
            areaName: "MailMimic",
            pattern: routePrefix + "/{controller}/{action=Index}/{id?}");

        return app;
    }
}
