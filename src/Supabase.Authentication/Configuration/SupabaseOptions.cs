using System.ComponentModel.DataAnnotations;

namespace Supabase.Authentication.Configuration;

/// <summary>
/// Configuration options for Supabase authentication with JWT signing keys.
/// </summary>
public class SupabaseOptions
{
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
    /// The JWT issuer for token validation.
    /// </summary>
    [Required]
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// The expected audience for JWT tokens.
    /// </summary>
    [Required]
    public string Audience { get; set; } = "authenticated";

    /// <summary>
    /// The JWKS endpoint URL for retrieving public keys.
    /// </summary>
    [Required]
    public string JwksUrl { get; set; } = string.Empty;
}