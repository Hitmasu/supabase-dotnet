namespace Supabase.Authentication.Auth.GoTrue.Requests;

public class SignUpRequest
{
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public object? Data { get; set; }
}