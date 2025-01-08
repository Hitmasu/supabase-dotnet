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
    public ushort Port => _dbContainer.GetMappedPublicPort(EnvVars["POSTGRES_PORT"]);
    public string PostgresDatabase => EnvVars["POSTGRES_DB"];

    /// <summary>
    /// External connection string to connect to the Postgres database outside from Docker.
    /// </summary>
    public string PostgresExternalConnectionString => GetConnectionString();

    /// <summary>
    /// Internal connection string to connect to the Postgres database inside from Docker.
    /// </summary>
    public string PostgresInternalConnectionString => GetConnectionString(true);

    /// <summary>
    /// Get connection string to connect to the Postgres database.
    /// </summary>
    /// <param name="isInternal"></param>
    /// <returns></returns>
    public string GetConnectionString(bool isInternal = false) =>
        $"Host={(isInternal ? "supabase-db" : "localhost")};" +
        $"Port={Port};" +
        $"Database={PostgresDatabase};" +
        $"Username={Username};" +
        $"Password={Password};";
}