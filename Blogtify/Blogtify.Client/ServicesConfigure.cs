using Bit.BlazorUI;
using Blazored.LocalStorage;
using Blogtify.Client.Services;
using Blogtify.Client.Shared.Services;
using Blogtify.Client.Theming;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Runtime.Intrinsics.Arm;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace Blogtify.Client;

public static class ServicesConfigure
{
    public static void AddCommonServices(
        this IServiceCollection services,
        IWebAssemblyHostEnvironment? wasmEnv = null,
        IConfiguration? config = null)
    {
        services.AddBitBlazorUIServices();
        services.AddScoped<IFontProvider, FontProvider>();
        services.AddBlazoredLocalStorage();
        services.AddHotKeys2();

        string? apiBase = null;
        if (wasmEnv != null)
        {
            if (string.IsNullOrEmpty(wasmEnv.BaseAddress))
                throw new InvalidOperationException("WASM BaseAddress is missing. Cannot configure HttpClient.");

            apiBase = wasmEnv.BaseAddress;
        }
        else
        {
            apiBase = config?["ApiBaseUrl"] ?? throw new ArgumentNullException("ApiBaseUrl is missing.");
        }

        services.AddSingleton<AppDataManager>(sp =>
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(apiBase)
            };
            return new AppDataManager(client);
        });

        services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri(apiBase)
        });

        services.AddScoped<ICommentService, CommentService>();
        
    }
}
