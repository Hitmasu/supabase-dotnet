using System.Text.Json.Serialization;
using Supabase.Common;

namespace Supabase.Authentication.Auth.GoTrue.Responses;

public class UserResponse<TCustomMetadata> where TCustomMetadata : UserMetadataBase
{
    public Guid Id { get; set; }
    public string Aud { get; set; }
    public string Role { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    
    public string BanDuration { get; set; }

    /// <summary>
    /// Only to update user password.
    /// </summary>
    public string Password { get; set; }

    [JsonPropertyName("email_confirmed_at")]
    public DateTime? EmailConfirmedAt { get; set; }

    [JsonPropertyName("phone_confirmed_at")]
    public DateTime? PhoneConfirmedAt { get; set; }

    [JsonPropertyName("last_sign_in_at")]
    public DateTime LastSignInAt { get; set; }

    [JsonPropertyName("user_metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TCustomMetadata UserMetadata { get; set; }

    [JsonPropertyName("app_metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AppMetadataResponse AppMetadata { get; set; }

    public IReadOnlyList<IdentityResponse<TCustomMetadata>> Identities { get; set; }
}