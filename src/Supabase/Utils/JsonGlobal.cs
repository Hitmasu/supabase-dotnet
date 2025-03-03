using System.Text.Json;

namespace Supabase.Utils;

public class JsonGlobal
{
    static JsonGlobal()
    {
        JsonSerializerOptions = new JsonSerializerOptions();
        JsonSerializerOptions.PropertyNameCaseInsensitive = true;   
    }
    
    public static JsonSerializerOptions JsonSerializerOptions { get; }
}