using System.Text.Json.Serialization;

namespace Supabase.Authentication.Auth.GoTrue.Responses;

public class InviteResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    
    [JsonPropertyName("confirmation_sent_at")]
    public DateTime ConfirmationSentAt { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    [JsonPropertyName("invited_at")]
    public DateTime InvitedAt { get; set; }
}