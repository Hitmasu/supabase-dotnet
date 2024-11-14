using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Supabase.Common.TokenResolver;

namespace Supabase;

public static class ServicesRegister
{
    public static SupabaseBuilder AddSupabase(this IServiceCollection services, string supabaseUrl, string apiKey) =>
        services.AddSupabase(new Uri(supabaseUrl), apiKey);

    public static SupabaseBuilder AddSupabase(this IServiceCollection services, Uri supabaseUrl, string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
            throw new ArgumentNullException(nameof(apiKey));

        var settings = new Settings(supabaseUrl, apiKey);

        services.AddLogging();

        services.AddSingleton(settings);

        var builder = new SupabaseBuilder()
        {
            Services = services,
            Settings = settings
        };
        
        return builder;
    }

    public static SupabaseBuilder AddSupabase<TTokenResolver>(this IServiceCollection services, Uri supabaseUrl,
        string apiKey) where TTokenResolver : class, ITokenResolver
    {
        services.AddScoped<ITokenResolver, TTokenResolver>();
        return services.AddSupabase(supabaseUrl, apiKey);
    }
}

public class LowerCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name) || !char.IsUpper(name[0]))
            return name;

        return name.ToLower();
    }
}