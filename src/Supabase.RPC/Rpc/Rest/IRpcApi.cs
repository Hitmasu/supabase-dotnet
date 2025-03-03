using Refit;
using Supabase.Utils.Attributes;

namespace Supabase.RPC.Rpc.Rest;

/// <summary>
/// Interface for Supabase RPC API
/// </summary>
public interface IRpcApi
{
    /// <summary>
    /// Calls a stored procedure with the specified name and parameters
    /// </summary>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="procedureName">The name of the stored procedure</param>
    /// <param name="parameters">The parameters to pass to the stored procedure</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>The result of the stored procedure</returns>
    [Post("/rest/v1/rpc/{procedureName}")]
    [RequiresAdmin]
    Task<TResult> CallAsync<TResult>(string procedureName, [Body] object parameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calls a stored procedure with the specified name and parameters
    /// </summary>
    /// <param name="procedureName">The name of the stored procedure</param>
    /// <param name="parameters">The parameters to pass to the stored procedure</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    [Post("/rest/v1/rpc/{procedureName}")]
    [RequiresAdmin]
    Task CallAsync(string procedureName, [Body] object parameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calls a stored procedure with the specified name and parameters in a specific schema
    /// </summary>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="procedureName">The name of the stored procedure</param>
    /// <param name="schema">The database schema name (passed as Accept-Profile header)</param>
    /// <param name="parameters">The parameters to pass to the stored procedure</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>The result of the stored procedure</returns>
    [Post("/rest/v1/rpc/{procedureName}")]
    [RequiresAdmin]
    Task<TResult> CallWithSchemaAsync<TResult>(string procedureName, [Header("Content-Profile")] string schema,
        [Body] object parameters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calls a stored procedure with the specified name and parameters in a specific schema
    /// </summary>
    /// <param name="procedureName">The name of the stored procedure</param>
    /// <param name="schema">The database schema name (passed as Accept-Profile header)</param>
    /// <param name="parameters">The parameters to pass to the stored procedure</param>
    /// <param name="cancellationToken">A token to cancel the operation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    [Post("/rest/v1/rpc/{procedureName}")]
    [Headers("Content-Profile: {schema}")]
    [RequiresAdmin]
    Task CallWithSchemaAsync(string procedureName, [Header("Content-Profile")] string schema, [Body] object parameters,
        CancellationToken cancellationToken = default);
}