namespace Supabase.Faker.Settings;

public class AuthenticationSettings : BaseSettings
{
    public AuthenticationSettings(Dictionary<string, string> envVars) : base(envVars)
    {
    }

    public string JwtSecret => EnvVars["JWT_SECRET"];
}