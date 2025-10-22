using Blogtify.Auth;
using Blogtify.Client;
using Blogtify.Client.Theming;
using Blogtify.Components;
using Blogtify.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var keyBase64 = builder.Configuration["DATAPROTECTION_KEY"];
var keyDir = new DirectoryInfo("/app/keys");
keyDir.Create();

if (!string.IsNullOrEmpty(keyBase64))
{
    var keyBytes = Convert.FromBase64String(keyBase64);
    File.WriteAllBytes(Path.Combine("/app/keys", "manual-key.xml"), keyBytes);
}

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(keyDir)
    .SetApplicationName("Blogtify");


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();


builder.Services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddOpenIdConnect(options =>
{
    builder.Configuration.Bind("Oidc", options);
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.ClaimActions.MapJsonKey("sub", "sub");
    options.ClaimActions.MapUniqueJsonKey(ClaimTypes.NameIdentifier, "sub");
    options.ClaimActions.MapUniqueJsonKey(ClaimTypes.Email, "email");
    options.ClaimActions.MapUniqueJsonKey("picture", "picture");

    options.TokenValidationParameters.NameClaimType = ClaimTypes.Email;
});

builder.Services.ConfigureCookieOidcRefresh(
    CookieAuthenticationDefaults.AuthenticationScheme,
    OpenIdConnectDefaults.AuthenticationScheme);

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

builder.Services.AddCommonServices(null, builder.Configuration);

builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IHttpContextProxy, ServerHttpContextProxy>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
builder.Services.AddAntiforgery();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseResponseCompression();
app.MapStaticAssets();

// Enable the .dat file extension (required to serve icudt.dat from _frameworkCompat/
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".dat"] = "application/octet-stream";
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Blogtify.Client._Imports).Assembly);

app.MapLoginAndLogout();
app.MapControllers();

app.Run();
