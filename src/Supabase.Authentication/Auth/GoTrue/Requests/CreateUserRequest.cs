using System.Text.Json.Serialization;
using Supabase.Common;

namespace Supabase.Authentication.Auth.GoTrue.Requests;

public class CreateUserRequest : CreateUserRequest<UserMetadataBase>
{
}

public class CreateUserRequest<TCustomMetadata> where TCustomMetadata : UserMetadataBase
{
    public string Role { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public bool EmailConfirm { get; set; }
    public bool PhoneConfirm { get; set; }
    
    [JsonPropertyName("user_metadata")]
    public TCustomMetadata UserMetadata { get; set; }
    
    [JsonPropertyName("app_metadata")]
    public AppMetadataRequest AppMetadata { get; set; }
    public string BanDuration { get; set; }
}