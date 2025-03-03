using Supabase.Common.TokenResolver;

namespace Supabase.Authentication.Tests;

internal class TokenResolver : ITokenResolver
{
    private static readonly AsyncLocal<TokenContext> CurrentContext = new();

    public void SetToken(string token)
    {
        var holder = CurrentContext.Value;

        if (holder != null)
            holder.Token = null;

        if (!string.IsNullOrEmpty(token))
            CurrentContext.Value = new TokenContext { Token = token };
    }

    public string? GetToken()
    {
        return CurrentContext.Value?.Token;
    }

    class TokenContext
    {
        public string Token { get; set; }
    }
}