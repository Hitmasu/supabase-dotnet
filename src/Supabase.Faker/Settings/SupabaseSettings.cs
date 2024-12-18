using DotNet.Testcontainers.Containers;

namespace Supabase.Faker.Settings;

public class SupabaseSettings : BaseSettings
{
    private readonly IContainer _kongContainer;

    public SupabaseSettings(IContainer kongContainer, Dictionary<string, string> envVars) : base(envVars)
    {
        _kongContainer = kongContainer;
    }

    public Uri Uri =>
        new UriBuilder(Uri.UriSchemeHttp, _kongContainer.Hostname, _kongContainer.GetMappedPublicPort(8000)).Uri;

    public string ServiceRoleKey => EnvVars["SERVICE_ROLE_KEY"];
}