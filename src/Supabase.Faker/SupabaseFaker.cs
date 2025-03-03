using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Supabase.Faker.Settings;
using Xunit;

namespace Supabase.Faker;

public class SupabaseFaker : IAsyncLifetime
{
    public bool IsRunning { get; private set; }
    private readonly IContainer _authContainer;
    private readonly IContainer _dbContainer;
    private readonly IContainer _kongContainer;
    private readonly INetwork _network;
    private readonly Dictionary<string, string> _envVars;
    private readonly IContainer _smtpContainer;
    private readonly IContainer _restContainer;
    private bool _disposed;
    private readonly string _dataPath;
    private readonly string _suffix;

    public string Name => "Supabase Faker";

    /// <summary>
    /// Settings from Supabase.
    /// </summary>
    [MemberNotNullWhen(true, nameof(IsRunning))]
    public SupabaseSettings? Supabase { get; private set; }

    /// <summary>
    /// Settingds from Supabase Authentication.
    /// </summary>
    [MemberNotNullWhen(true, nameof(IsRunning))]
    public AuthenticationSettings? Authentication { get; private set; }

    /// <summary>
    /// Settings from Supabase Postgres.
    /// </summary>
    [MemberNotNullWhen(true, nameof(IsRunning))]
    public PostgresSettings? Postgres { get; private set; }

    /// <summary>
    /// Settings from Supabase REST API (PostgREST).
    /// </summary>
    [MemberNotNullWhen(true, nameof(IsRunning))]
    public RestSettings? Rest { get; private set; }

    public SupabaseFaker(bool shouldReuse = false, FakerConfig? config = null)
    {
        config ??= new FakerConfig();
        _suffix = shouldReuse ? string.Empty : $"-{Guid.NewGuid().ToString("N").Substring(0, 8)}";

        _dataPath = Path.Combine(Path.GetTempPath(), "supabase-data");
        Directory.CreateDirectory(_dataPath);

        ExtractSupabaseFiles().Wait();

        _envVars = LoadEnvironmentVariables();

        config.Envs = _envVars;

        _network = new NetworkBuilder()
            .WithReuse(shouldReuse)
            .WithName($"supabase-network{_suffix}")
            .Build();

        _dbContainer = new ContainerBuilder()
            .WithReuse(shouldReuse)
            .WithImage("supabase/postgres:15.1.1.78")
            .WithName($"supabase-db{_suffix}")
            .WithNetwork(_network)
            .WithNetworkAliases("db", "supabase-db", "database")
            .WithEnvironment("POSTGRES_PASSWORD", _envVars["POSTGRES_PASSWORD"])
            .WithEnvironment("POSTGRES_DB", _envVars["POSTGRES_DB"])
            .WithEnvironment("POSTGRES_PORT", _envVars["POSTGRES_PORT"])
            .WithEnvironment("JWT_SECRET", _envVars["JWT_SECRET"])
            .WithEnvironment("JWT_EXPIRY", _envVars["JWT_EXPIRY"])
            .WithPortBinding(int.Parse(_envVars["POSTGRES_PORT"]), true)
            .WithBindMount($"{_dataPath}/volumes/db/realtime.sql",
                "/docker-entrypoint-initdb.d/migrations/99-realtime.sql")
            .WithBindMount($"{_dataPath}/volumes/db/webhooks.sql",
                "/docker-entrypoint-initdb.d/init-scripts/98-webhooks.sql")
            .WithBindMount($"{_dataPath}/volumes/db/roles.sql", "/docker-entrypoint-initdb.d/init-scripts/99-roles.sql")
            .WithBindMount($"{_dataPath}/volumes/db/jwt.sql", "/docker-entrypoint-initdb.d/init-scripts/99-jwt.sql")
            .WithBindMount($"{_dataPath}/volumes/db/_supabase.sql",
                "/docker-entrypoint-initdb.d/migrations/97-_supabase.sql")
            .WithBindMount($"{_dataPath}/volumes/db/logs.sql", "/docker-entrypoint-initdb.d/migrations/99-logs.sql")
            .WithBindMount($"{_dataPath}/volumes/db/pooler.sql", "/docker-entrypoint-initdb.d/migrations/99-pooler.sql")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilCommandIsCompleted("pg_isready", "-U", "postgres", "-h", "localhost"))
            .Build();

        _authContainer = new ContainerBuilder()
            .WithReuse(shouldReuse)
            .WithImage("supabase/gotrue:v2.158.1")
            .WithName($"supabase-auth{_suffix}")
            .WithNetwork(_network)
            .WithNetworkAliases("auth", "supabase-auth")
            .WithEnvironment("GOTRUE_DB_DATABASE_URL",
                $"postgres://supabase_auth_admin:{_envVars["POSTGRES_PASSWORD"]}@supabase-db:{_envVars["POSTGRES_PORT"]}/{_envVars["POSTGRES_DB"]}")
            .WithEnvironment("GOTRUE_API_HOST", "0.0.0.0")
            .WithEnvironment("GOTRUE_API_PORT", "9999")
            .WithEnvironment("API_EXTERNAL_URL", _envVars["API_EXTERNAL_URL"])
            .WithEnvironment("GOTRUE_DB_DRIVER", "postgres")
            .WithEnvironment("GOTRUE_SITE_URL", _envVars["SITE_URL"])
            .WithEnvironment("GOTRUE_URI_ALLOW_LIST", _envVars["ADDITIONAL_REDIRECT_URLS"])
            .WithEnvironment("GOTRUE_DISABLE_SIGNUP", _envVars["DISABLE_SIGNUP"])
            .WithEnvironment("GOTRUE_JWT_ADMIN_ROLES", "service_role")
            .WithEnvironment("GOTRUE_JWT_AUD", "authenticated")
            .WithEnvironment("GOTRUE_JWT_DEFAULT_GROUP_NAME", "authenticated")
            .WithEnvironment("GOTRUE_JWT_EXP", _envVars["JWT_EXPIRY"])
            .WithEnvironment("GOTRUE_JWT_SECRET", _envVars["JWT_SECRET"])
            .WithEnvironment("GOTRUE_EXTERNAL_EMAIL_ENABLED", _envVars["ENABLE_EMAIL_SIGNUP"])
            .WithEnvironment("GOTRUE_EXTERNAL_ANONYMOUS_USERS_ENABLED", _envVars["ENABLE_ANONYMOUS_USERS"])
            .WithEnvironment("GOTRUE_MAILER_AUTOCONFIRM", _envVars["ENABLE_EMAIL_AUTOCONFIRM"])
            .WithEnvironment("GOTRUE_SMTP_ADMIN_EMAIL", _envVars["SMTP_ADMIN_EMAIL"])
            .WithEnvironment("GOTRUE_SMTP_HOST", _envVars["SMTP_HOST"])
            .WithEnvironment("GOTRUE_SMTP_PORT", _envVars["SMTP_PORT"])
            .WithEnvironment("GOTRUE_SMTP_USER", _envVars["SMTP_USER"])
            .WithEnvironment("GOTRUE_SMTP_PASS", _envVars["SMTP_PASS"])
            .WithEnvironment("GOTRUE_SMTP_SENDER_NAME", _envVars["SMTP_SENDER_NAME"])
            .WithEnvironment("GOTRUE_MAILER_URLPATHS_INVITE", _envVars["MAILER_URLPATHS_INVITE"])
            .WithEnvironment("GOTRUE_MAILER_URLPATHS_CONFIRMATION", _envVars["MAILER_URLPATHS_CONFIRMATION"])
            .WithEnvironment("GOTRUE_MAILER_URLPATHS_RECOVERY", _envVars["MAILER_URLPATHS_RECOVERY"])
            .WithEnvironment("GOTRUE_MAILER_URLPATHS_EMAIL_CHANGE", _envVars["MAILER_URLPATHS_EMAIL_CHANGE"])
            .WithEnvironment("GOTRUE_EXTERNAL_PHONE_ENABLED", _envVars["ENABLE_PHONE_SIGNUP"])
            .WithEnvironment("GOTRUE_SMS_AUTOCONFIRM", _envVars["ENABLE_PHONE_AUTOCONFIRM"])
            .WithPortBinding(9999, true)
            .Build();


        _restContainer = new ContainerBuilder()
            .WithReuse(shouldReuse)
            .WithImage("postgrest/postgrest:v12.2.0")
            .WithName($"supabase-rest{_suffix}")
            .WithNetwork(_network)
            .WithNetworkAliases("rest", "supabase-rest")
            .WithEnvironment("PGRST_DB_URI",
                $"postgres://authenticator:{_envVars["POSTGRES_PASSWORD"]}@supabase-db:{_envVars["POSTGRES_PORT"]}/{_envVars["POSTGRES_DB"]}")
            .WithEnvironment("PGRST_DB_SCHEMAS", config.GetRpcSchemas)
            .WithEnvironment("PGRST_DB_ANON_ROLE", "anon")
            .WithEnvironment("PGRST_JWT_SECRET", _envVars["JWT_SECRET"])
            .WithEnvironment("PGRST_DB_USE_LEGACY_GUCS", "false")
            .WithEnvironment("PGRST_APP_SETTINGS_JWT_SECRET", _envVars["JWT_SECRET"])
            .WithEnvironment("PGRST_APP_SETTINGS_JWT_EXP", _envVars["JWT_EXPIRY"])
            .WithPortBinding(3000, true)
            .Build();

        _kongContainer = new ContainerBuilder()
            .WithReuse(shouldReuse)
            .WithImage("kong:2.8.1")
            .WithName($"supabase-kong{_suffix}")
            .WithNetwork(_network)
            .WithNetworkAliases("kong", "api")
            .WithEnvironment("KONG_DATABASE", "off")
            .WithEnvironment("KONG_DECLARATIVE_CONFIG", "/home/kong/kong.yml")
            .WithEnvironment("KONG_DNS_ORDER", "LAST,A,CNAME")
            .WithEnvironment("KONG_DNS_RESOLVER", "127.0.0.11")
            .WithEnvironment("KONG_PLUGINS", "request-transformer,cors,key-auth,acl,basic-auth")
            .WithEnvironment("KONG_NGINX_PROXY_PROXY_BUFFER_SIZE", "160k")
            .WithEnvironment("KONG_NGINX_PROXY_PROXY_BUFFERS", "64 160k")
            .WithEnvironment("SUPABASE_ANON_KEY", _envVars["ANON_KEY"])
            .WithEnvironment("SUPABASE_SERVICE_KEY", _envVars["SERVICE_ROLE_KEY"])
            .WithEnvironment("DASHBOARD_USERNAME", _envVars["DASHBOARD_USERNAME"])
            .WithEnvironment("DASHBOARD_PASSWORD", _envVars["DASHBOARD_PASSWORD"])
            .WithPortBinding(int.Parse(_envVars["KONG_HTTP_PORT"]), true)
            .WithPortBinding(int.Parse(_envVars["KONG_HTTPS_PORT"]), true)
            .WithBindMount($"{_dataPath}/volumes/api/kong.yml", "/home/kong/temp.yml")
            .WithEntrypoint("/bin/bash", "-c", @"
                cp /home/kong/temp.yml /home/kong/kong.yml && \
                sed -i 's|\$SUPABASE_ANON_KEY|'$SUPABASE_ANON_KEY'|g; \
                        s|\$SUPABASE_SERVICE_KEY|'$SUPABASE_SERVICE_KEY'|g; \
                        s|\$DASHBOARD_USERNAME|'$DASHBOARD_USERNAME'|g; \
                        s|\$DASHBOARD_PASSWORD|'$DASHBOARD_PASSWORD'|g' /home/kong/kong.yml && \
                /docker-entrypoint.sh kong docker-start")
            .Build();

        _smtpContainer = new ContainerBuilder()
            .WithReuse(shouldReuse)
            .WithImage("gessnerfl/fake-smtp-server:2.4.1")
            .WithName($"supabase-smtp{_suffix}")
            .WithNetwork(_network)
            .WithNetworkAliases("smtp", "supabase-mail")
            .WithPortBinding(5080, true)
            .WithPortBinding(8025, true)
            .WithPortBinding(8080, true)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request =>
                    request.ForPath("/api/emails")
                        .ForPort(8080)
                        .ForResponseMessageMatching(async response => response.StatusCode == HttpStatusCode.OK)
                ))
            .Build();
    }

    private Dictionary<string, string> LoadEnvironmentVariables()
    {
        var envVars = new Dictionary<string, string>();
        var envPath = Path.Combine(_dataPath, ".env");

        foreach (var line in File.ReadAllLines(envPath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#')) continue;

            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                envVars[parts[0].Trim()] = parts[1].Trim();
            }
        }

        return envVars;
    }

    private async Task ExtractSupabaseFiles()
    {
        var assembly = Assembly.GetExecutingAssembly();
        const string fileName = "Supabase.zip";

        var assemblyName = typeof(SupabaseFaker).Assembly.GetName().Name;
        var resourceName = $"{assemblyName}.{fileName}";

        await using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
            throw new InvalidOperationException($"{fileName} resource not found");

        using var archive = new ZipArchive(stream);
        archive.ExtractToDirectory(_dataPath, true);
    }

    [MemberNotNull(nameof(Postgres))]
    [MemberNotNull(nameof(Authentication))]
    [MemberNotNull(nameof(Supabase))]
    [MemberNotNull(nameof(Rest))]
    public async Task InitializeAsync()
    {
        await _network.CreateAsync();
        await _dbContainer.StartAsync();
        await _authContainer.StartAsync();
        await _restContainer.StartAsync();
        await _kongContainer.StartAsync();
        await _smtpContainer.StartAsync();

        Postgres = new PostgresSettings(_envVars, _dbContainer);
        Authentication = new AuthenticationSettings(_envVars);
        Supabase = new SupabaseSettings(_kongContainer, _envVars);
        Rest = new RestSettings(_restContainer, _envVars);

        IsRunning = true;
    }

    public async Task DisposeAsync()
    {
        if (_disposed) return;

        await _authContainer.DisposeAsync();
        await _dbContainer.DisposeAsync();
        await _kongContainer.DisposeAsync();
        await _restContainer.DisposeAsync();
        await _network.DisposeAsync();
        await _smtpContainer.DisposeAsync();

        if (Directory.Exists(_dataPath))
            Directory.Delete(_dataPath, true);

        _disposed = true;
        IsRunning = false;
    }
}