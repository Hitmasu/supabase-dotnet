using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using Refit;
using Supabase.Authentication.Auth;
using Supabase.Authentication.Auth.GoTrue;
using Supabase.Authentication.Auth.JwtSigningKeys;
using Supabase.Authentication.Configuration;
using Supabase.Clients.Handlers;
using Supabase.Common.TokenResolver;
using Supabase.Utils;
using Supabase.Utils.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Supabase.Authentication;

public static class ServicesRegister
{
    public static SupabaseAuthenticationBuilder AddSupabaseAuthentication(
       this SupabaseBuilder builder,
       IConfiguration configuration,
       string configSectionName = "Supabase")
    {
        var services = builder.Services;
        services.AddScoped<ISupabaseAuth, SupabaseAuth>();
        services.AddRefit(builder.Settings.Url);

        var supabaseOptions = configuration.GetSection(configSectionName).Get<SupabaseOptions>();

        if (supabaseOptions == null)
            throw new InvalidOperationException($"Supabase configuration section '{configSectionName}' not found");
        
        builder.Settings.Issuer = supabaseOptions.Issuer;
        builder.Settings.Audience = supabaseOptions.Audience;
        builder.Settings.JwksUrl = supabaseOptions.JwksUrl;
        builder.Settings.EnableAsymmetricKeys = true;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var configManager = new ConfigurationManager<JsonWebKeySet>(
                builder.Settings.JwksUrl,
                new SupabaseJwksRetriever(),
                new HttpDocumentRetriever()
            );

            var random = new Random();
            var jitter = TimeSpan.FromMinutes(random.Next(0, 30));

            configManager.RefreshInterval = TimeSpan.FromHours(1).Add(jitter);
            configManager.AutomaticRefreshInterval = TimeSpan.FromMinutes(30).Add(jitter);

            options.TokenValidationParameters = CreateValidationParameters(builder.Settings, configManager);
        });

        services.AddAuthorization();

        var authBuilder = new SupabaseAuthenticationBuilder()
        {
            Services = services,
            Settings = builder.Settings
        };

        return authBuilder;
    }

    [Obsolete("Use AddSupabaseAuthentication with signing keys instead of JWT secret. This method will be removed in a future version.")]
    public static SupabaseAuthenticationBuilder AddSupabaseAuthentication(this SupabaseBuilder builder,
        string jwtSecret,
        JwtBearerEvents? events = null)
    {
        var services = builder.Services;
        services.AddScoped<ISupabaseAuth, SupabaseAuth>();
        services.AddRefit(builder.Settings.Url);

        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(o =>
            {
                o.IncludeErrorDetails = true;
                o.SaveToken = true;

                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSecret)
                    ),
                    ValidateIssuer = false,
                    ValidateAudience = true,
                    ValidAudience = "authenticated",
                };

                if (events != null)
                    o.Events = events;
            });

        var authBuilder = new SupabaseAuthenticationBuilder()
        {
            Services = services,
            Settings = builder.Settings
        };

        return authBuilder;
    }

    public static IServiceCollection WithUserInjection<TUser>(this SupabaseAuthenticationBuilder builder,
        JsonSerializerOptions? jsonOptions = null)
        where TUser : class
    {
        var services = builder.Services;

        return services.AddScoped<TUser>(provider =>
        {
            var tokenResolver = provider.GetRequiredService<ITokenResolver>();
            var token = tokenResolver.GetToken();

            if (token.IsNullOrEmpty())
                return null;

            var handler = new JwtSecurityTokenHandler();
            var jwtTokenObj = handler.ReadJwtToken(token);

            var userMetadata = jwtTokenObj.Claims.FirstOrDefault(c => c.Type == "user_metadata")!.Value;

            if (userMetadata == null)
                throw new Exception("User metadata not found in JWT.");

            var options = jsonOptions ?? JsonGlobal.JsonSerializerOptions;
            var user = JsonSerializer.Deserialize<TUser>(userMetadata, options);

            return user!;
        });
    }

    private static TokenValidationParameters CreateValidationParameters(
        Settings settings,
        ConfigurationManager<JsonWebKeySet> configManager)
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidIssuer = settings.Issuer,
            ValidateAudience = true,
            ValidAudiences = [settings.Audience, "authenticated"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ValidAlgorithms = [SecurityAlgorithms.EcdsaSha256],
            IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
            {
                var keySet = configManager.GetConfigurationAsync(CancellationToken.None).GetAwaiter().GetResult();
                var keys = keySet.GetSigningKeys();

                if (!string.IsNullOrEmpty(kid))
                    return keys.Where(k => k.KeyId == kid);

                return keys;
            },
            ConfigurationManager = configManager
        };
    }

    private static void AddRefit(this IServiceCollection services, Uri supabaseUrl)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new LowerCaseNamingPolicy(),
            Converters =
            {
                new JsonStringEnumConverter(new LowerCaseNamingPolicy())
            }
        };

        var authUrl = new Uri($"{supabaseUrl}auth/v1");

        services.AddTransient<AuthHandler>();
        services.AddTransient<ErrorHandler>();

        services.AddRefitClient<IGoTrueApi>(_ =>
                new RefitSettings(new SystemTextJsonContentSerializer(jsonOptions)))
            .ConfigureHttpClient(c => c.BaseAddress = authUrl)
            .AddHttpMessageHandler<AuthHandler>()
            .AddHttpMessageHandler<ErrorHandler>();
    }
}