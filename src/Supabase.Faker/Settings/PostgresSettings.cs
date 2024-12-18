using DotNet.Testcontainers.Containers;

namespace Supabase.Faker.Settings;

public class PostgresSettings : BaseSettings
{
    private readonly IContainer _dbContainer;

    public PostgresSettings(Dictionary<string, string> envVars, IContainer dbContainer) : base(envVars)
    {
        _dbContainer = dbContainer;
    }

    public string Username => "supabase_auth_admin";
    public string Password => EnvVars["POSTGRES_PASSWORD"];
    public string Host => "supabase-db";
    public ushort Port => _dbContainer.GetMappedPublicPort(EnvVars["POSTGRES_PORT"]);
    public string PostgresDatabase => EnvVars["POSTGRES_DB"];

    public string PostgresConnectionString =>
        $"postgres://{Username}:{Password}@{Host}:{Port}/{PostgresDatabase}";
}