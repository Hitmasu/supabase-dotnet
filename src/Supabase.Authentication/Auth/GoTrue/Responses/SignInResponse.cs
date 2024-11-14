using System.Text.Json.Serialization;
using Supabase.Common;

namespace Supabase.Authentication.Auth.GoTrue.Responses;

public class SignInResponse<TCustomMetadata> where TCustomMetadata : UserMetadataBase
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public ulong ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }

    public UserResponse<TCustomMetadata> User { get; set; }
}