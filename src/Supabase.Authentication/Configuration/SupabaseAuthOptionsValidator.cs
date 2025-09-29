using Microsoft.Extensions.Options;

namespace Supabase.Authentication.Configuration;

/// <summary>
/// Validator for SupabaseAuthOptions.
/// </summary>
public class SupabaseAuthOptionsValidator : IValidateOptions<SupabaseAuthOptions>
{
    public ValidateOptionsResult Validate(string? name, SupabaseAuthOptions options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Url))
            errors.Add("Supabase URL is required");

        if (string.IsNullOrWhiteSpace(options.ApiKey))
            errors.Add("Supabase API Key is required");

        if (!Uri.TryCreate(options.Url, UriKind.Absolute, out _))
            errors.Add("Supabase URL must be a valid absolute URI");        

        if (!options.EnableAsymmetricKeys && string.IsNullOrWhiteSpace(options.JwtSecret))
            errors.Add("JWT Secret is required when asymmetric keys are disabled");

        if (options.ClockSkew < TimeSpan.Zero)
            errors.Add("Clock skew must be non-negative");

        if (options.JwksRefreshInterval <= TimeSpan.Zero)
            errors.Add("JWKS refresh interval must be positive");

        if (options.JwksAutoRefreshInterval <= TimeSpan.Zero)
            errors.Add("JWKS auto refresh interval must be positive");

        return errors.Any()
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}