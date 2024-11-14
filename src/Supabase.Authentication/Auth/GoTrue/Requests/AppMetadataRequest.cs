namespace Supabase.Authentication.Auth.GoTrue.Requests;

public class AppMetadataRequest
{
    public string Provider { get; set; }
    public IReadOnlyList<string> Providers { get; set; }
}