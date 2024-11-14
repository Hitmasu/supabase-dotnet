using System.Text.Json.Serialization;

namespace Supabase.Common.Errors;

/// <summary>
/// Error response from Supabase API.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Code of the error.
    /// </summary>
    public virtual int Code { get; set; }
    
    /// <summary>
    /// Description of code.
    /// </summary>
    [JsonPropertyName("error_code")]
    public virtual string Error { get; set; }

    /// <summary>
    /// Message of the error.
    /// </summary>
    [JsonPropertyName("msg")]
    public virtual string Message { get; set; }
}