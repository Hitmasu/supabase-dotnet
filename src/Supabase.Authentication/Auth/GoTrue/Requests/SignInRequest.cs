namespace Supabase.Authentication.Auth.GoTrue.Requests;

/// <summary>
/// Request to signup on Supabase.
/// </summary>
public class SignInRequest
{
    /// <summary>
    /// E-mail user.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Phone user.
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// Password from user.
    /// </summary>
    public string Password { get; set; }
}