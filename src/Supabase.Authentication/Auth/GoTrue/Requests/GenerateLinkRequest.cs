using System.Text.Json.Serialization;
using Supabase.Authentication.Auth.GoTrue.Enums;
using Supabase.Common;

namespace Supabase.Authentication.Auth.GoTrue.Requests;

public class GenerateLinkRequest<TCustomMetadata> where TCustomMetadata : UserMetadataBase
{
    public ActionType Type { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public TCustomMetadata Data { get; set; }

    [JsonPropertyName("redirect_to")]
    public string RedirectTo { get; set; }
}