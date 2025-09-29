namespace Supabase;

/// <summary>
/// Supabase settings with JWT signing keys support.
/// </summary>
public class Settings
{
    /// <summary>
    /// The Supabase project URL.
    /// </summary>
    public Uri Url { get; set; }

    /// <summary>
    /// The Supabase API key (anon key).
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// The JWT issuer for token validation.
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// The expected audience for JWT tokens.
    /// </summary>
    public string Audience { get; set; }

    /// <summary>
    /// The JWKS endpoint URL for retrieving public keys.
    /// </summary>
    public string JwksUrl { get; set; }

    /// <summary>
    /// Whether to enable asymmetric JWT key validation.
    /// </summary>
    public bool EnableAsymmetricKeys { get; set; }

    public Settings(Uri url, string apiKey)
    {
        Url = url;
        ApiKey = apiKey;
        EnableAsymmetricKeys = false;
    }

    public Settings(Uri url, string apiKey, string issuer, string audience, string jwksUrl, bool enableAsymmetricKeys = true)
    {
        Url = url;
        ApiKey = apiKey;
        Issuer = issuer;
        Audience = audience;
        JwksUrl = jwksUrl;
        EnableAsymmetricKeys = enableAsymmetricKeys;
    }

    /// <summary>
    /// Creates Settings from URL and ApiKey, inferring other properties from the URL.
    /// </summary>
    /// <param name="url">The Supabase project URL</param>
    /// <param name="apiKey">The Supabase API key</param>
    /// <param name="audience">The JWT audience (default: "authenticated")</param>
    /// <param name="enableAsymmetricKeys">Whether to enable asymmetric keys (default: true)</param>
    /// <returns>A new Settings instance</returns>
    public static Settings FromUrl(Uri url, string apiKey, string audience = "authenticated", bool enableAsymmetricKeys = true)
    {
        var baseUrl = url.ToString().TrimEnd('/');
        var issuer = $"{baseUrl}/auth/v1";
        var jwksUrl = $"{baseUrl}/auth/v1/.well-known/jwks.json";

        return new Settings(url, apiKey, issuer, audience, jwksUrl, enableAsymmetricKeys);
    }

    /// <summary>
    /// Creates Settings from URL string and ApiKey, inferring other properties.
    /// </summary>
    /// <param name="url">The Supabase project URL as string</param>
    /// <param name="apiKey">The Supabase API key</param>
    /// <param name="audience">The JWT audience (default: "authenticated")</param>
    /// <param name="enableAsymmetricKeys">Whether to enable asymmetric keys (default: true)</param>
    /// <returns>A new Settings instance</returns>
    public static Settings FromUrl(string url, string apiKey, string audience = "authenticated", bool enableAsymmetricKeys = true)
    {
        return FromUrl(new Uri(url), apiKey, audience, enableAsymmetricKeys);
    }
}