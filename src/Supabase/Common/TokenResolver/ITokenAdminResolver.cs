namespace Supabase.Common.TokenResolver;

/// <summary>
/// Token resolver for admin Supabase.
/// </summary>
public interface ITokenAdminResolver
{
    /// <summary>
    /// Resolve which token to use.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>Token JWT to use.</returns>
    string GetToken();
}