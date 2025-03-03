using DotNet.Testcontainers.Containers;

namespace Supabase.Faker.Settings;

/// <summary>
/// Settings for Supabase REST API (PostgREST).
/// </summary>
public class RestSettings : BaseSettings
{
    private readonly IContainer _restContainer;

    public RestSettings(IContainer restContainer, Dictionary<string, string> envVars) : base(envVars)
    {
        _restContainer = restContainer;
    }

    /// <summary>
    /// The URI to connect to the REST API.
    /// </summary>
    public Uri Uri =>
        new UriBuilder(Uri.UriSchemeHttp, _restContainer.Hostname, _restContainer.GetMappedPublicPort(3000)).Uri;

    /// <summary>
    /// The database schemas exposed by the REST API.
    /// </summary>
    public string Schemas => EnvVars.ContainsKey("PGRST_DB_SCHEMAS") 
        ? EnvVars["PGRST_DB_SCHEMAS"] 
        : "public,storage,graphql_public";

    /// <summary>
    /// The anonymous role used by the REST API.
    /// </summary>
    public string AnonRole => "anon";

    /// <summary>
    /// The JWT secret used by the REST API.
    /// </summary>
    public string JwtSecret => EnvVars["JWT_SECRET"];
} 