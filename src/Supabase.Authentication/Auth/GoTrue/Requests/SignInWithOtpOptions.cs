using System.Text.Json.Serialization;

namespace Supabase.Authentication.Auth.GoTrue.Requests;

public class SignInWithOtpOptions
{
    [JsonPropertyName("email_redirect_to")]
    public string? EmailRedirectTo { get; set; }

    [JsonPropertyName("should_create_user")]
    public bool ShouldCreateUser { get; set; } = true;

    [JsonPropertyName("data")]
    public object? Data { get; set; }
}
