using Supabase.Clients;

namespace Supabase.RPC.Rpc;

/// <summary>
/// Interface for Supabase RPC client
/// </summary>
public interface ISupabaseRpc : IClientBase
{
    /// <summary>
    /// Calls a stored procedure with the specified name and parameters
    /// </summary>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="procedureName">The name of the stored procedure</param>
    /// <param name="parameters">The parameters to pass to the stored procedure</param>
    /// <param name="schema">Optional schema name to call the procedure from</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>The result of the stored procedure</returns>
    Task<TResult> CallAsync<TResult>(string procedureName, object parameters, string? schema = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calls a stored procedure with the specified name and parameters
    /// </summary>
    /// <param name="procedureName">The name of the stored procedure</param>
    /// <param name="parameters">The parameters to pass to the stored procedure</param>
    /// <param name="schema">Optional schema name to call the procedure from</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task CallAsync(string procedureName, object parameters, string? schema = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calls a stored procedure with the specified name and parameters
    /// </summary>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="procedureName">The name of the stored procedure</param>
    /// <param name="schema">Optional schema name to call the procedure from</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>The result of the stored procedure</returns>
    Task<TResult> CallAsync<TResult>(string procedureName, string? schema = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a schema-specific RPC client for the specified schema
    /// </summary>
    /// <param name="schema">The database schema name</param>
    /// <returns>A schema-specific RPC client</returns>
    SupabaseRpc.SchemaRpcClient ForSchema(string schema);
} 