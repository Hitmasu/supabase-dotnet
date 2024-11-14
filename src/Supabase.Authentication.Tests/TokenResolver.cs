using Npgsql;
using Supabase.Common;
using Supabase.Common.TokenResolver;

namespace Supabase.Authentication.Tests;

public class TokenResolver : ITokenResolver
{
    public Func<string> GetTokenDel { get; set; }

    public string GetToken()
    {
        return GetTokenDel();
    }
}