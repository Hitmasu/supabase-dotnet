namespace Supabase.Authentication.Auth.GoTrue.Requests;

public class InviteRequest
{
    public string Email { get; set; }
    public object? Data { get; set; }

    public InviteRequest(string email, object? data)
    {
        Email = email;
        Data = data;
    }
}