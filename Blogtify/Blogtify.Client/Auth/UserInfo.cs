using System.Security.Claims;

namespace Blogtify.Client.Auth;

public class UserInfo
{
    public string Sub { get; set; } = default!;
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Picture { get; set; }

    public static UserInfo FromClaimsPrincipal(ClaimsPrincipal principal)
    {
        var subClaim = principal.FindFirst("sub") ?? principal.FindFirst(ClaimTypes.NameIdentifier)
                       ?? principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        if (subClaim == null)
        {
            throw new InvalidOperationException("Could not find required 'sub' claim.");
        }

        var email = principal.FindFirst(ClaimTypes.Email)?.Value
                    ?? principal.FindFirst("email")?.Value;

        var name = principal.FindFirst("name")?.Value
                   ?? principal.FindFirst(ClaimTypes.GivenName)?.Value
                   ?? principal.FindFirst(ClaimTypes.Surname)?.Value;

        var picture = principal.FindFirst("picture")?.Value;

        return new UserInfo
        {
            Sub = subClaim.Value,
            Email = email,
            Name = name,
            Picture = picture
        };
    }

    public ClaimsPrincipal ToClaimsPrincipal()
    {
        var claims = new List<Claim>();

        if (!string.IsNullOrEmpty(Sub))
            claims.Add(new Claim("sub", Sub));
        if (!string.IsNullOrEmpty(Email))
            claims.Add(new Claim(ClaimTypes.Email, Email));
        if (!string.IsNullOrEmpty(Name))
            claims.Add(new Claim(ClaimTypes.Name, Name));
        if (!string.IsNullOrEmpty(Picture))
            claims.Add(new Claim("picture", Picture));

        var identity = new ClaimsIdentity(claims, "oidc");
        return new ClaimsPrincipal(identity);
    }


    public static string? GetClaimFallback(ClaimsPrincipal principal, params string[] types)
    {
        foreach (var t in types)
        {
            var c = principal.FindFirst(t);
            if (c != null) return c.Value;
        }
        return null;
    }
}