using Microsoft.Extensions.DependencyInjection;
using Refit;
using Supabase.Clients.Handlers;
using Supabase.RPC.Rpc;
using Supabase.RPC.Rpc.Rest;

namespace Supabase.RPC;

/// <summary>
/// Extensions for SupabaseBuilder
/// </summary>
public static class ServicesRegister
{
    /// <summary>
    /// Adds the RPC client to the Supabase builder
    /// </summary>
    /// <param name="builder">The Supabase builder</param>
    /// <returns>The Supabase builder</returns>
    public static SupabaseBuilder AddSupabaseRpc(this SupabaseBuilder builder)
    {
        var settings = builder.Settings;
        var services = builder.Services;

        services.AddTransient<AuthHandler>();
        services.AddTransient<ErrorHandler>();

        services.AddRefitClient<IRpcApi>()
            .ConfigureHttpClient(c => c.BaseAddress = settings.Url)
            .AddHttpMessageHandler<AuthHandler>()
            .AddHttpMessageHandler<ErrorHandler>();

        services.AddScoped<ISupabaseRpc, SupabaseRpc>();

        return builder;
    }
} 