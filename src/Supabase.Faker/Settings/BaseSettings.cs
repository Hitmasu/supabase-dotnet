namespace Supabase.Faker.Settings;

public abstract class BaseSettings
{
    private static string? _cachedDockerHost;

    public BaseSettings(Dictionary<string, string> envVars)
    {
        EnvVars = envVars;
    }

    protected Dictionary<string, string> EnvVars { get; }

    protected static string ResolveDockerHost(string fallbackHost)
    {
        var configuredHost = Environment.GetEnvironmentVariable("TESTCONTAINERS_HOST_OVERRIDE");
        if (!string.IsNullOrWhiteSpace(configuredHost))
            return configuredHost;

        if (!string.Equals(fallbackHost, "localhost", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(fallbackHost, "127.0.0.1", StringComparison.OrdinalIgnoreCase))
        {
            return fallbackHost;
        }

        if (!string.IsNullOrWhiteSpace(_cachedDockerHost))
            return _cachedDockerHost;

        const string routePath = "/proc/net/route";
        if (!File.Exists(routePath))
            return fallbackHost;

        try
        {
            foreach (var line in File.ReadAllLines(routePath).Skip(1))
            {
                var fields = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (fields.Length <= 2 || fields[1] != "00000000")
                    continue;

                var gatewayHex = fields[2];
                if (gatewayHex.Length != 8)
                    continue;

                var octets = Enumerable.Range(0, 4)
                    .Select(index => Convert.ToInt32(gatewayHex.Substring(6 - (index * 2), 2), 16))
                    .ToArray();

                _cachedDockerHost = string.Join('.', octets);
                return _cachedDockerHost;
            }
        }
        catch
        {
            return fallbackHost;
        }

        return fallbackHost;
    }
}
