using Microsoft.Extensions.Logging;
using Supabase.Clients;
using Supabase.RPC.Rpc.Rest;

namespace Supabase.RPC.Rpc;

/// <summary>
/// Client for Supabase RPC
/// </summary>
public class SupabaseRpc : ClientBase<SupabaseRpc>, ISupabaseRpc
{
    private readonly IRpcApi _rpcApi;

    /// <summary>
    /// Initializes a new instance of the <see cref="SupabaseRpc"/> class
    /// </summary>
    /// <param name="rpcApi">The RPC API</param>
    /// <param name="logger">The logger</param>
    public SupabaseRpc(IRpcApi rpcApi, ILogger<SupabaseRpc> logger) : base(logger)
    {
        _rpcApi = rpcApi;
    }

    /// <inheritdoc />
    public async Task<TResult> CallAsync<TResult>(string procedureName, object parameters, string? schema = null,
        CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Calling RPC procedure {ProcedureName} with parameters {Parameters} in schema {Schema}", procedureName,
            parameters, schema ?? "public");

        var result = schema == null 
            ? await _rpcApi.CallAsync<TResult>(procedureName, parameters, cancellationToken)
            : await _rpcApi.CallWithSchemaAsync<TResult>(procedureName, schema, parameters, cancellationToken);
            
        Logger.LogDebug("RPC procedure {ProcedureName} called successfully", procedureName);
        return result;
    }

    /// <inheritdoc />
    public async Task CallAsync(string procedureName, object parameters, string? schema = null, 
        CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Calling RPC procedure {ProcedureName} with parameters {Parameters} in schema {Schema}", procedureName,
            parameters, schema ?? "public");

        if (schema == null)
            await _rpcApi.CallAsync(procedureName, parameters, cancellationToken);
        else
            await _rpcApi.CallWithSchemaAsync(procedureName, schema, parameters, cancellationToken);
            
        Logger.LogDebug("RPC procedure {ProcedureName} called successfully", procedureName);
    }

    /// <inheritdoc />
    public async Task<TResult> CallAsync<TResult>(string procedureName, string? schema = null, 
        CancellationToken cancellationToken = default)
    {
        // Call with empty parameters
        return await CallAsync<TResult>(procedureName, new { }, schema, cancellationToken);
    }
    
    /// <summary>
    /// Creates a schema-specific RPC client for the specified schema
    /// </summary>
    /// <param name="schema">The database schema name</param>
    /// <returns>A schema-specific RPC client</returns>
    public SchemaRpcClient ForSchema(string schema)
    {
        return new SchemaRpcClient(this, schema);
    }
    
    /// <summary>
    /// Schema-specific RPC client that simplifies calling procedures in a specific schema
    /// </summary>
    public class SchemaRpcClient
    {
        private readonly SupabaseRpc _rpcClient;
        private readonly string _schema;
        
        internal SchemaRpcClient(SupabaseRpc rpcClient, string schema)
        {
            _rpcClient = rpcClient;
            _schema = schema;
        }
        
        /// <summary>
        /// Calls a RPC procedure in this schema with a result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="procedureName">The procedure name</param>
        /// <param name="parameters">The parameters</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The result of the procedure</returns>
        public async Task<TResult> CallAsync<TResult>(string procedureName, object parameters,
            CancellationToken cancellationToken = default)
        {
            return await _rpcClient.CallAsync<TResult>(procedureName, parameters, _schema, cancellationToken);
        }
        
        /// <summary>
        /// Calls a RPC procedure in this schema without a result
        /// </summary>
        /// <param name="procedureName">The procedure name</param>
        /// <param name="parameters">The parameters</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task CallAsync(string procedureName, object parameters,
            CancellationToken cancellationToken = default)
        {
            await _rpcClient.CallAsync(procedureName, parameters, _schema, cancellationToken);
        }
        
        /// <summary>
        /// Calls a RPC procedure in this schema with a result and no parameters
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="procedureName">The procedure name</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The result of the procedure</returns>
        public async Task<TResult> CallAsync<TResult>(string procedureName,
            CancellationToken cancellationToken = default)
        {
            return await _rpcClient.CallAsync<TResult>(procedureName, _schema, cancellationToken);
        }
    }
}