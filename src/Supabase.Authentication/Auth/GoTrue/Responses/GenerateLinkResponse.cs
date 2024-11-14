using System.Text.Json.Serialization;
using Supabase.Authentication.Auth.GoTrue.Enums;
using Supabase.Common;

namespace Supabase.Authentication.Auth.GoTrue.Responses;

public class GenerateLinkResponse<TCustomMetadata> : UserResponse<TCustomMetadata>
    where TCustomMetadata : UserMetadataBase
{
    [JsonPropertyName("action_link")]
    public string ActionLink { get; set; }

    [JsonPropertyName("email_otp")]
    public string EmailOtp { get; set; }

    [JsonPropertyName("hashed_token")]
    public string HashedToken { get; set; }

    [JsonPropertyName("verification_type")]
    public ActionType Type { get; set; }

    [JsonPropertyName("redirect_to")]
    public string RedirectTo { get; set; }
}