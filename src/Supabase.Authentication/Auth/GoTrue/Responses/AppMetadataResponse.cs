namespace Supabase.Authentication.Auth.GoTrue.Responses;

public class AppMetadataResponse
{
    public string Provider { get; set; }
    public IReadOnlyList<string> Providers { get; set; }
}