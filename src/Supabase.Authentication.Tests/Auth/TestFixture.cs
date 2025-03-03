using Microsoft.Extensions.DependencyInjection;
using Supabase.Common.TokenResolver;
using Supabase.Faker;

namespace Supabase.Authentication.Tests.Auth;

public class TestFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; set; }

    public TestFixture()
    {
        var faker = new SupabaseFaker();
        faker.InitializeAsync().Wait();
        
        var collection = new ServiceCollection();
        
        collection.AddSupabase(faker.Supabase.Uri, faker.Supabase.ServiceRoleKey)
            .AddSupabaseAuthentication(faker.Authentication.JwtSecret);
        
        collection.AddScoped<ITokenResolver, TokenResolver>();
        
        ServiceProvider = collection.BuildServiceProvider();
    }

    public void Dispose()
    {
    }
}