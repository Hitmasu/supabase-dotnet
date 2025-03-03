using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Refit;
using Supabase.Authentication.Auth;
using Supabase.Authentication.Auth.GoTrue;
using Supabase.Clients.Handlers;
using Supabase.Common.TokenResolver;
using Supabase.Utils;
using Supabase.Utils.Extensions;

namespace Supabase.Authentication;

public static class ServicesRegister
{
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