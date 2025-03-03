namespace Supabase.Faker;

public class FakerConfig
{
    internal Dictionary<string, string> Envs { get; set; }
    public IReadOnlyList<string>? RpcSchemas { get; set; }

    internal string GetRpcSchemas => RpcSchemas == null ? Envs["PGRST_DB_SCHEMAS"] : string.Join(",", RpcSchemas);
}