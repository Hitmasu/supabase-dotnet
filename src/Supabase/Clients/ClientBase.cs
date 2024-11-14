using Microsoft.Extensions.Logging;

namespace Supabase.Clients;

public abstract class ClientBase<TClient> : IClientBase
{
    protected ILogger<TClient> Logger { get; }

    protected ClientBase(ILogger<TClient> logger)
    {
        Logger = logger;
    }
}