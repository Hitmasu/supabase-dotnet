using System.Text.Json.Serialization;

namespace Supabase.Authentication.Auth.GoTrue.Requests;

public class VerifyOtpRequest
{
    [JsonPropertyName("token_hash")]
    public string TokenHash { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "email";
}
