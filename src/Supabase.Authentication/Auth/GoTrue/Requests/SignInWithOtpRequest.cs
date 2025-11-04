using System.Text.Json.Serialization;

namespace Supabase.Authentication.Auth.GoTrue.Requests;

public class SignInWithOtpRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("create_user")]
    public bool CreateUser { get; set; } = true;

    [JsonPropertyName("data")]
    public object? Data { get; set; }
}
