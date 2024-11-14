using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Supabase.Common.TokenResolver;

namespace Supabase.Clients.Handlers;

/// <summary>
/// Handler to add the Authorization header to the request.
/// </summary>
public class AuthHandler : DelegatingHandler
{
    private readonly Settings _settings;
    private readonly IServiceProvider? _serviceProvider;

    public AuthHandler(IServiceProvider? serviceProvider, Settings settings)
    {
        _serviceProvider = serviceProvider;
        _settings = settings;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Add("apikey", _settings.ApiKey);

        var tokenResolver = _serviceProvider!.GetService<ITokenResolver>();
        
        if (tokenResolver != null)
        {
            var token = tokenResolver.GetToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}