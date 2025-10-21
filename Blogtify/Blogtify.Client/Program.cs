using Blogtify.Client;
using Blogtify.Client.Auth;
using Blogtify.Client.Models;
using Blogtify.Client.Theming;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


using var response = await new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
}.GetAsync("appsettings.json");

using var stream = await response.Content.ReadAsStreamAsync();
builder.Configuration.AddJsonStream(stream);

builder.Services.Configure<AdSenseSettings>(
    builder.Configuration.GetSection("AdSense"));

builder.Services.AddScoped<IHttpContextProxy, WebAssemblyHttpContextProxy>();

builder.Services.AddCommonServices(builder.HostEnvironment, null);

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();


await builder.Build().RunAsync();