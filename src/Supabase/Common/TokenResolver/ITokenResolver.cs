namespace Supabase.Common.TokenResolver;

/// <summary>
/// Token resolver for Supabase.
/// </summary>
public interface ITokenResolver
{
    /// <summary>
    /// Resolve which token to use.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>Token JWT to use.</returns>
    string GetToken();
}