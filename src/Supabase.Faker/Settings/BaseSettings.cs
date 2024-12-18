namespace Supabase.Faker.Settings;

public abstract class BaseSettings
{
    public BaseSettings(Dictionary<string, string> envVars)
    {
        EnvVars = envVars;
    }

    protected Dictionary<string, string> EnvVars { get; }
}