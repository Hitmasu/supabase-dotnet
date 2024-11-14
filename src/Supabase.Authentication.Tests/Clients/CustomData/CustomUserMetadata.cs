using Supabase.Common;

namespace Supabase.Authentication.Tests.Clients.CustomData;

public class CustomUserMetadata : UserMetadataBase
{
    public Address Address { get; set; }
}