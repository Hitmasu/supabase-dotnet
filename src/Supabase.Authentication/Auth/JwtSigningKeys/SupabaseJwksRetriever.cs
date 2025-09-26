using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;

namespace Supabase.Authentication.Auth.JwtSigningKeys;

/// <summary>
/// JWKS retriever for Supabase authentication.
/// </summary>
public class SupabaseJwksRetriever : IConfigurationRetriever<JsonWebKeySet>
{
    /// <summary>
    /// Retrieves JWKS configuration from the specified address.
    /// </summary>
    /// <param name="address">The JWKS endpoint URL.</param>
    /// <param name="retriever">The document retriever.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The JSON Web Key Set.</returns>
    public async Task<JsonWebKeySet> GetConfigurationAsync(
        string address,
        IDocumentRetriever retriever,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var document = await retriever.GetDocumentAsync(address, cancellationToken);
            return new JsonWebKeySet(document);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unable to retrieve JWKS from {address}: {ex.Message}", ex);
        }
    }
}