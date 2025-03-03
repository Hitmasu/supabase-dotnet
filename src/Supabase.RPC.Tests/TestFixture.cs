using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Supabase.Common.TokenResolver;
using Supabase.Faker;

namespace Supabase.RPC.Tests;

/// <summary>
/// Fixture for RPC tests
/// </summary>
/// <remarks>
/// Scenario: Configuration of the test environment for RPC functions
/// 
/// Expected behavior:
/// 1. Initializes the Supabase environment with SupabaseFaker
/// 2. Executes SQL scripts to create the necessary RPC functions
/// 3. Configures the services needed for the tests
/// 
/// Verifications:
/// - All RPC functions are created in the database
/// - Services are correctly configured
/// </remarks>
public class TestFixture : IDisposable
{
    internal IServiceProvider ServiceProvider { get; set; }
    private static SupabaseFaker _faker;
    private static bool _isLoaded;
    private static readonly Lock FakerLock = new();

    public TestFixture()
    {
        lock (FakerLock)
        {
            if (!_isLoaded)
            {
                var fakerConfig = new FakerConfig
                {
                    RpcSchemas = ["public", "auth"]
                };

                _faker = new SupabaseFaker(false,fakerConfig);
                _faker.InitializeAsync().Wait();

                // Execute SQL scripts to create RPC functions
                ExecuteRpcScripts().Wait();

                _isLoaded = true;
            }
        }

        var collection = new ServiceCollection();

        collection.AddSupabase(_faker.Supabase.Uri, _faker.Supabase.ServiceRoleKey)
            .AddSupabaseRpc();

        collection.AddScoped<ITokenResolver, TokenResolver>();

        ServiceProvider = collection.BuildServiceProvider();
    }

    /// <summary>
    /// Executes SQL scripts to create RPC functions
    /// </summary>
    private async Task ExecuteRpcScripts()
    {
        var connectionString =
            _faker.Postgres.PostgresExternalConnectionString.Replace("supabase_auth_admin", "postgres");

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        // List of scripts to execute
        var scriptFiles = new[]
        {
            "setup_functions.sql",
            "setup_auth_functions.sql",
            "setup_error_functions.sql",
            "setup_complex_types_functions.sql"
        };

        foreach (var scriptFile in scriptFiles)
        {
            try
            {
                var script = await ReadEmbeddedResourceAsync(scriptFile);

                await using var command = new NpgsqlCommand(script, connection);
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing script {scriptFile}: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// Reads an embedded resource from the assembly
    /// </summary>
    private async Task<string> ReadEmbeddedResourceAsync(string scriptName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{assembly.GetName().Name}.Scripts.{scriptName}";

        await using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            // Try to find the resource by listing all available resources
            var resources = assembly.GetManifestResourceNames();

            var matchingResource = resources.FirstOrDefault(r => r.EndsWith(scriptName));

            if (matchingResource != null)
            {
                await using var matchStream = assembly.GetManifestResourceStream(matchingResource);
                if (matchStream != null)
                {
                    using var reader = new StreamReader(matchStream);
                    return await reader.ReadToEndAsync();
                }
            }

            // If not found as an embedded resource, try to read from the file system
            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", scriptName);

            if (File.Exists(fullPath))
            {
                return await File.ReadAllTextAsync(fullPath);
            }

            throw new FileNotFoundException(
                $"Resource not found: {resourceName}. Available resources: {string.Join(", ", resources)}");
        }

        using var streamReader = new StreamReader(stream);
        return await streamReader.ReadToEndAsync();
    }

    public void Dispose()
    {
        _faker.DisposeAsync().Wait();
    }
}

public class TokenResolver : ITokenResolver
{
    private string _token = string.Empty;

    public string GetToken() => _token;

    public void SetToken(string token)
    {
        _token = token;
    }
}