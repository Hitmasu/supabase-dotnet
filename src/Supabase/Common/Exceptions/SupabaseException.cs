using Supabase.Common.Errors;

namespace Supabase.Common.Exceptions;

/// <summary>
/// Exception base from Supabase.
/// </summary>
public class SupabaseException : Exception
{
    /// <summary>
    /// Information about error returned from Supabase.
    /// </summary>
    public ErrorResponse Info { get; set; }
    
    /// <summary>
    /// Content returned from Supabase.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Default supabase exception.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="info"></param>
    public SupabaseException(string message, ErrorResponse info, string content) : base(message)
    {
        Info = info;
        Content = content;
    }
}