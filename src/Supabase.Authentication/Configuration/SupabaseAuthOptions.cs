using System.ComponentModel.DataAnnotations;

namespace Supabase.Authentication.Configuration;

/// <summary>
/// Configuration options for Supabase authentication.
/// </summary>
public class SupabaseAuthOptions
{
    public const string SectionName = "Supabase";

    /// <summary>
    /// The Supabase project URL.
    /// </summary>
    [Required]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// The Supabase API key (anon key).
    /// </summary>
    [Required]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The JWT secret for symmetric validation (fallback).
    /// </summary>
    public string? JwtSecret { get; set; }

    /// <summary>
    /// Whether to enable asymmetric JWT key validation.
    /// </summary>
    public bool EnableAsymmetricKeys { get; set; } = true;

    /// <summary>
    /// The JWKS endpoint URL.
    /// </summary>
    public string JwksUrl => $"{Url.TrimEnd('/')}/auth/v1/.well-known/jwks.json";

    /// <summary>
    /// The JWT issuer for token validation.
    /// </summary>
    public string Issuer => $"{Url.TrimEnd('/')}/auth/v1";

    /// <summary>
    /// The expected audience for JWT tokens.
    /// </summary>
    public string Audience { get; set; } = "authenticated";

    /// <summary>
    /// Additional valid audiences.
    /// </summary>
    public string[] ValidAudiences { get; set; } = ["authenticated"];

    /// <summary>
    /// The algorithms allowed for JWT validation.
    /// </summary>
    public string[] ValidAlgorithms { get; set; } = ["ES256"];

    /// <summary>
    /// Clock skew tolerance for token validation.
    /// </summary>
    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Whether to require expiration time in tokens.
    /// </summary>
    public bool RequireExpirationTime { get; set; } = true;

    /// <summary>
    /// Whether to require signed tokens.
    /// </summary>
    public bool RequireSignedTokens { get; set; } = true;

    /// <summary>
    /// JWKS refresh interval.
    /// </summary>
    public TimeSpan JwksRefreshInterval { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// JWKS automatic refresh interval.
    /// </summary>
    public TimeSpan JwksAutoRefreshInterval { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Whether to add random jitter to refresh intervals.
    /// </summary>
    public bool EnableRefreshJitter { get; set; } = true;

    /// <summary>
    /// Maximum jitter time to add to refresh intervals.
    /// </summary>
    public TimeSpan MaxRefreshJitter { get; set; } = TimeSpan.FromMinutes(30);
}