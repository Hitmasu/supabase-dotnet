namespace Supabase;

public class Settings
{
    public Uri Url { get; set; }
    public string ApiKey { get; set; }

    public Settings(Uri url, string apiKey)
    {
        Url = url;
        ApiKey = apiKey;
    }
}