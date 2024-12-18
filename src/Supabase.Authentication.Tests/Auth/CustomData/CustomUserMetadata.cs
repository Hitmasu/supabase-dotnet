using Supabase.Common;

namespace Supabase.Authentication.Tests.Auth.CustomData;

public class CustomUserMetadata : UserMetadataBase
{
    public Address Address { get; set; }
}