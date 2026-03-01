using System.Text.Json.Serialization;
using Supabase.Common;

namespace Supabase.Authentication.Auth.GoTrue.Responses;

public class IdentityResponse<TCustomMetadata> where TCustomMetadata : UserMetadataBase
{
    /// <summary>
    /// Provider user identifier (for example, Google subject), not a UUID.
    /// </summary>
    public string Id { get; set; }

    [JsonPropertyName("identity_id")]
    public Guid IdentityId { get; set; }

    [JsonPropertyName("user_id")]
    public Guid UserId { get; set; }

    public string Provider { get; set; }

    [JsonPropertyName("identity_data")]
    public TCustomMetadata IdentityData { get; set; }

    [JsonPropertyName("last_sign_in_at")]
    public DateTime LastSignInAt { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    public string Email { get; set; }
}
