using System.Security.Claims;
using System.Text.Encodings.Web;
using AiAgent.Api.Domain.Database.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AiAgent.Api.Auth;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly IApiKeyRepository _apiKeyRepository;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IApiKeyRepository apiKeyRepository) 
        : base(options, logger, encoder, clock)
    {
        _apiKeyRepository = apiKeyRepository;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(Options.HeaderName))
        {
            return AuthenticateResult.NoResult();
        }

        string? apiKey = Request.Headers[Options.HeaderName];

        if (string.IsNullOrEmpty(apiKey))
        {
            return AuthenticateResult.NoResult();
        }

        var existingApiKey = await _apiKeyRepository.GetByKeyAsync(apiKey);

        if (existingApiKey == null || (existingApiKey.Expires.HasValue && existingApiKey.Expires.Value < DateTime.UtcNow))
        {
            return AuthenticateResult.Fail("Invalid API Key.");
        }

        var claims = new[] { new Claim(ClaimTypes.Name, existingApiKey.Owner) };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
