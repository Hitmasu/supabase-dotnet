using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Supabase.Common.TokenResolver;
using Supabase.Utils.Attributes;

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

        var token = GetTokenAuthentication(request);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }

    private string GetTokenAuthentication(HttpRequestMessage request)
    {
        var restMethodInfo = request.Properties[HttpRequestMessageOptions.RestMethodInfo] as RestMethodInfo;
        var requiresAdmin = restMethodInfo?.MethodInfo.GetCustomAttribute<RequiresAdminAttribute>() != null;

        if (requiresAdmin)
        {
            var tokenAdminResolver = _serviceProvider!.GetService<ITokenAdminResolver>();

            if (tokenAdminResolver != null)
                return tokenAdminResolver.GetToken();

            return _settings.ApiKey;
        }

        var tokenResolver = _serviceProvider!.GetService<ITokenResolver>();
        return tokenResolver.GetToken();
    }
}