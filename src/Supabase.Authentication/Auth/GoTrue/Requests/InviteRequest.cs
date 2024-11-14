namespace Supabase.Authentication.Auth.GoTrue.Requests;

public class InviteRequest
{
    public string Email { get; set; }

    public InviteRequest(string email)
    {
        Email = email;
    }
}