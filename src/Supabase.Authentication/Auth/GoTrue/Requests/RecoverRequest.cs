namespace Supabase.Authentication.Auth.GoTrue.Requests;

public class RecoverRequest
{
    public string Email { get; set; }

    public RecoverRequest(string email)
    {
        Email = email;
    }
}