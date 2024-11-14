using System.Text.Json.Serialization;

namespace Supabase.Authentication.Auth.GoTrue.Responses;

public class SettingsResponse
{
    public External External { get; set; }

    [JsonPropertyName("disable_signup")]
    public bool DisableSignup { get; set; }

    public bool AutoConfirm { get; set; }
}

public class External
{
    public bool Apple { get; set; }
    public bool Azure { get; set; }
    public bool Bitbucket { get; set; }
    public bool Discord { get; set; }
    public bool Facebook { get; set; }
    public bool Figma { get; set; }
    public bool Github { get; set; }
    public bool Gitlab { get; set; }
    public bool Google { get; set; }
    public bool Keycloak { get; set; }
    public bool Linkedin { get; set; }
    public bool Notion { get; set; }
    public bool Slack { get; set; }
    public bool Spotify { get; set; }
    public bool Twitch { get; set; }
    public bool Twitter { get; set; }
    public bool Workos { get; set; }
}