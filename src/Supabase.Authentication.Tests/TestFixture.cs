using Microsoft.Extensions.DependencyInjection;
using Supabase.Common.TokenResolver;
using Supabase.Faker;

namespace Supabase.Authentication.Tests;

public class TestFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; set; }
    private readonly SupabaseFaker _faker;

    public TestFixture()
    {
        _faker = new SupabaseFaker();
        _faker.InitializeAsync().Wait();
        
        var collection = new ServiceCollection();
        
        collection.AddSupabase(_faker.Supabase.Uri, _faker.Supabase.ServiceRoleKey)
            .AddSupabaseAuthentication(_faker.Authentication.JwtSecret);
        
        collection.AddScoped<ITokenResolver, TokenResolver>();
        
        ServiceProvider = collection.BuildServiceProvider();
    }

    public void Dispose()
    {
        _faker.DisposeAsync().Wait();
    }
}