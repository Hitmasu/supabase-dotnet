using System.Text.Json;
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

        ErrorResponse errorInfo;

        try
        {
            errorInfo = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            })!;
        }
        catch
        {
            errorInfo = new ErrorResponse()
            {
                InternalMessage = responseContent
            };
        }

        throw new SupabaseException(errorInfo.Message, errorInfo, responseContent);
    }
}