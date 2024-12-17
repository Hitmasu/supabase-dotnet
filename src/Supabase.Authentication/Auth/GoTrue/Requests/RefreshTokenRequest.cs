using System.Text.Json.Serialization;

namespace Supabase.Authentication.Auth.GoTrue.Requests;

public class RefreshTokenRequest
{
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }

    public RefreshTokenRequest(string refreshToken)
    {
        RefreshToken = refreshToken;
    }
}