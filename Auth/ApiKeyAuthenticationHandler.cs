using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace SchoolStaffAPI.Auth;

public class SecretAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration configuration)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var value))
            return Task.FromResult(AuthenticateResult.Fail("Authorization header not found."));

        var authHeader = value.ToString();
        if (!authHeader.StartsWith("Secret ", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(AuthenticateResult.Fail("Authorization scheme must be 'Secret'."));

        var secret = authHeader["Secret ".Length..].Trim();
        var configuredSecret = configuration["Secret"];

        if (string.IsNullOrEmpty(configuredSecret))
            return Task.FromResult(AuthenticateResult.Fail("API Key is not configured."));

        if (!configuredSecret.Equals(secret))
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key."));

        var claims = new[] { new Claim("ApiAccess", "true") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
} 