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
    [JsonIgnore]
    public virtual string Error => (ErrorCode ?? InternalErrorCode)!;

    [JsonIgnore]
    public string Message => (InternalMessage ?? ErrorDescription)!;

    /// <summary>
    /// Message of the error.
    /// </summary>
    [JsonPropertyName("msg")]
    public virtual string? InternalMessage { get; set; }

    [JsonPropertyName("error_description")]
    public string? ErrorDescription { get; set; }

    [JsonPropertyName("error_code")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("error")]
    public string? InternalErrorCode { get; set; }
}