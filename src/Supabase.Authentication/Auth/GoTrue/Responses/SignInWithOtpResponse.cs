using System.Text.Json.Serialization;

namespace Supabase.Authentication.Auth.GoTrue.Responses;

public class SignInWithOtpResponse
{
    [JsonPropertyName("message_id")]
    public string? MessageId { get; set; }
}
