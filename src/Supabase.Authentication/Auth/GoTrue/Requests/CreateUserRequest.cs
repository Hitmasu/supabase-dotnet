using System.Text.Json.Serialization;
using Supabase.Common;

namespace Supabase.Authentication.Auth.GoTrue.Requests;

public class CreateUserRequest : CreateUserRequest<UserMetadataBase>
{
}

public class CreateUserRequest<TCustomMetadata> where TCustomMetadata : UserMetadataBase
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("phone")]
    public string Phone { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("email_confirm")]
    public bool EmailConfirm { get; set; }

    [JsonPropertyName("phone_confirm")]
    public bool PhoneConfirm { get; set; }

    [JsonPropertyName("user_metadata")]
    public TCustomMetadata UserMetadata { get; set; }

    [JsonPropertyName("app_metadata")]
    public AppMetadataRequest AppMetadata { get; set; }

    [JsonPropertyName("ban_duration")]
    public string BanDuration { get; set; }
}