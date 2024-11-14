using Microsoft.Extensions.DependencyInjection;

namespace Supabase;

public class SupabaseBuilder
{
    public IServiceCollection Services { get; set; }
    public Settings Settings { get; set; }
}