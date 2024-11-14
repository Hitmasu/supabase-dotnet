using System.Net.Http.Json;
using System.Text.Json;
using Supabase.Common;
using Supabase.Common.Errors;
using Supabase.Common.Exceptions;

namespace Supabase.Clients.Handlers;

/// <summary>
/// Handler to handle errors from the Supabase API.
/// </summary>
public class ErrorHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode)
            return response;

        var responseContent = await response.Content.ReadAsStringAsync();
        var errorInfo = JsonSerializer.Deserialize<ErrorResponse>(responseContent)!;

        throw new SupabaseException(errorInfo.Message, errorInfo, responseContent);
    }
}