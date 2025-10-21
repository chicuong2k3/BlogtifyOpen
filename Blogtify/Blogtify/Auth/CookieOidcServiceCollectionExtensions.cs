using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Blogtify.Auth;

internal static class CookieOidcServiceCollectionExtensions
{
    public static IServiceCollection ConfigureCookieOidcRefresh(this IServiceCollection services, string cookieScheme, string oidcScheme)
    {
        services.AddSingleton<CookieOidcRefresher>();
        services.AddOptions<CookieAuthenticationOptions>(cookieScheme).Configure<CookieOidcRefresher>((cookieOptions, refresher) =>
        {
            cookieOptions.Events.OnValidatePrincipal = context => refresher.ValidateOrRefreshCookieAsync(context, oidcScheme);
        });

        services.AddOptions<OpenIdConnectOptions>(oidcScheme).Configure(oidcOptions =>
        {
            var provider = oidcOptions.Authority ?? "";

            // Chỉ thêm offline_access nếu không phải Google
            if (!provider.Contains("accounts.google.com", StringComparison.OrdinalIgnoreCase))
            {
                oidcOptions.Scope.Add(OpenIdConnectScope.OfflineAccess);
            }

            oidcOptions.SaveTokens = true;
        });

        return services;
    }
}
