using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Supabase.Authentication.Auth;
using Supabase.Authentication.Auth.GoTrue.Enums;
using Supabase.Authentication.Auth.GoTrue.Requests;
using Supabase.Authentication.Tests.Clients.CustomData;
using Supabase.Common.Exceptions;
using Supabase.Common.TokenResolver;

namespace Supabase.Authentication.Tests.Clients;

public class SupabaseAuth
{
    [Fact]
    public async Task SignUpAsync_ValidParameters_ReturnsUser()
    {
        var client = GetAuthClient();
        var faker = new Faker();
        var email = faker.Internet.Email().ToLower();
        var password = faker.Internet.Password();

        var createdUser = await client.SignUpAsync(email, password);

        createdUser.AccessToken.Should().NotBeNullOrEmpty();
        createdUser.TokenType.Should().NotBeNullOrEmpty();
        createdUser.ExpiresIn.Should().BeGreaterThan(0);
        createdUser.RefreshToken.Should().NotBeNullOrEmpty();
        createdUser.User.Should().NotBeNull();

        var user = createdUser.User;

        user.Id.Should().NotBeEmpty();
        user.Aud.Should().NotBeNullOrEmpty();
        user.Role.Should().NotBeNullOrEmpty();

        user.Email.Should().Be(email);
        user.Phone.Should().BeNullOrEmpty();

        user.EmailConfirmedAt.Should().NotBe(default);
        user.PhoneConfirmedAt.Should().BeNull();
        user.LastSignInAt.Should().NotBe(default);

        user.AppMetadata.Should().NotBeNull();
        user.AppMetadata.Provider.Should().Be("email");
        user.AppMetadata.Providers.Should().Contain("email");

        user.UserMetadata.Should().NotBeNull();
        user.UserMetadata.Email.Should().Be(email);
        user.UserMetadata.EmailVerified.Should().BeFalse();
        user.UserMetadata.PhoneVerified.Should().BeFalse();
        user.UserMetadata.Sub.Should().NotBeNullOrEmpty();

        user.Identities.Should().NotBeNull();

        foreach (var identity in user.Identities)
        {
            identity.Id.Should().NotBeEmpty();
            identity.IdentityId.Should().NotBeEmpty();
            identity.UserId.Should().NotBeEmpty();
            identity.IdentityData.Should().NotBeNull();
            identity.Provider.Should().Be("email");
            identity.LastSignInAt.Should().NotBe(default);
            identity.CreatedAt.Should().NotBe(default);
            identity.UpdatedAt.Should().NotBe(default);
            identity.Email.Should().Be(email);
        }
    }

    [Fact]
    public async Task SignUpAsync_WithCustomData_ReturnUser()
    {
        var client = GetAuthClient();
        var faker = new Faker();
        var email = faker.Internet.Email().ToLower();
        var password = faker.Internet.Password();

        var address = new Address()
        {
            Street = faker.Address.StreetName(),
            Apartment = faker.Address.BuildingNumber(),
            City = faker.Address.City(),
            Country = faker.Address.Country(),
            State = faker.Address.State(),
            ZipCode = faker.Address.ZipCode()
        };

        var customData = new CustomUserMetadata()
        {
            Address = address
        };

        var createdUser = await client.SignUpAsync(email, password, customData);
        var user = createdUser.User;
        user.UserMetadata.Address.Should().NotBeNull();
        user.UserMetadata.Address.Should().BeEquivalentTo(address);
        user.UserMetadata.Should().NotBeNull();
        user.UserMetadata.Email.Should().Be(email);
        user.UserMetadata.EmailVerified.Should().BeFalse();
        user.UserMetadata.PhoneVerified.Should().BeFalse();
        user.UserMetadata.Sub.Should().NotBeNullOrEmpty();

        var identity = user.Identities[0];
        identity.IdentityData.Address.Should().NotBeNull();
        identity.IdentityData.Address.Should().BeEquivalentTo(address);
    }

    [Fact]
    public async Task SignUpWithPhoneAsync_ValidParameters_ReturnsUser()
    {
        var client = GetAuthClient();
        var faker = new Faker();
        var phone = faker.Phone.PhoneNumber("55##########");
        var password = faker.Internet.Password();

        var createdUser = await client.SignUpWithPhoneAsync(phone, password);

        createdUser.AccessToken.Should().NotBeNullOrEmpty();
        createdUser.TokenType.Should().NotBeNullOrEmpty();
        createdUser.ExpiresIn.Should().BeGreaterThan(0);
        createdUser.RefreshToken.Should().NotBeNullOrEmpty();
        createdUser.User.Should().NotBeNull();

        var user = createdUser.User;

        user.Id.Should().NotBeEmpty();
        user.Aud.Should().NotBeNullOrEmpty();
        user.Role.Should().NotBeNullOrEmpty();

        user.Email.Should().BeNullOrEmpty();
        user.Phone.Should().Be(phone);

        user.EmailConfirmedAt.Should().BeNull();
        user.PhoneConfirmedAt.Should().NotBe(default);
        user.LastSignInAt.Should().NotBe(default);

        user.AppMetadata.Should().NotBeNull();
        user.AppMetadata.Provider.Should().Be("phone");
        user.AppMetadata.Providers.Should().Contain("phone");

        user.UserMetadata.Should().NotBeNull();
        user.UserMetadata.Email.Should().BeNullOrEmpty();
        user.UserMetadata.EmailVerified.Should().BeFalse();
        user.UserMetadata.PhoneVerified.Should().BeFalse();
        user.UserMetadata.Sub.Should().NotBeNullOrEmpty();

        user.Identities.Should().NotBeNull();

        foreach (var identity in user.Identities)
        {
            identity.Id.Should().NotBeEmpty();
            identity.IdentityId.Should().NotBeEmpty();
            identity.UserId.Should().NotBeEmpty();
            identity.IdentityData.Should().NotBeNull();
            identity.Provider.Should().Be("phone");
            identity.LastSignInAt.Should().NotBe(default);
            identity.CreatedAt.Should().NotBe(default);
            identity.UpdatedAt.Should().NotBe(default);
            identity.Email.Should().BeNullOrEmpty();
        }
    }

    [Fact]
    public async Task SignUpWithPhoneAsync_WithCustomData_ReturnsUser()
    {
        var client = GetAuthClient();
        var faker = new Faker();
        var phone = faker.Phone.PhoneNumber("55##########");
        var password = faker.Internet.Password();

        var address = new Address()
        {
            Street = faker.Address.StreetName(),
            Apartment = faker.Address.BuildingNumber(),
            City = faker.Address.City(),
            Country = faker.Address.Country(),
            State = faker.Address.State(),
            ZipCode = faker.Address.ZipCode()
        };

        var customData = new CustomUserMetadata()
        {
            Address = address
        };

        var createdUser = await client.SignUpWithPhoneAsync(phone, password, customData);
        var user = createdUser.User;
        user.UserMetadata.Address.Should().NotBeNull();
        user.UserMetadata.Address.Should().BeEquivalentTo(address);
        user.UserMetadata.Should().NotBeNull();
        user.UserMetadata.Email.Should().BeNullOrEmpty();
        user.UserMetadata.EmailVerified.Should().BeFalse();
        user.UserMetadata.PhoneVerified.Should().BeFalse();
        user.UserMetadata.Sub.Should().NotBeNullOrEmpty();

        var identity = user.Identities[0];
        identity.IdentityData.Address.Should().NotBeNull();
        identity.IdentityData.Address.Should().BeEquivalentTo(address);
    }

    [Fact]
    public async Task SignUpAsync_EmptyEmail_ThrowsArgumentNullException()
    {
        var client = GetAuthClient();
        var faker = new Faker();
        var password = faker.Internet.Password();

        var action = async () => await client.SignUpAsync(string.Empty, password);

        var ex = await action.Should().ThrowAsync<ArgumentNullException>();
        ex.WithMessage("E-mail or Phone cannot be null or empty.");
    }

    [Fact]
    public async Task SignUpAsync_EmptyPassword_ThrowsArgumentNullException()
    {
        var client = GetAuthClient();
        var faker = new Faker();
        var email = faker.Internet.Email().ToLower();

        var action = async () => await client.SignUpAsync(email, string.Empty);

        var ex = await action.Should().ThrowAsync<ArgumentNullException>();
        ex.WithMessage("Password cannot be null or empty. (Parameter 'Password')");
    }

    [Fact]
    public async Task SignInAsync_ValidParameters_ReturnsAccessToken()
    {
        var client = GetAuthClient();
        var faker = new Faker();
        var email = faker.Internet.Email().ToLower();
        var password = faker.Internet.Password();

        await client.SignUpAsync(email, password);

        var accessToken = await client.SignInAsync(email, password);

        accessToken.Should().NotBeNull();

        accessToken.AccessToken.Should().NotBeNullOrEmpty();
        accessToken.TokenType.Should().NotBeNullOrEmpty();
        accessToken.ExpiresIn.Should().BeGreaterThan(0);
        accessToken.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SignInAsync_WithCustomData_ReturnsAccessTokenWithUser()
    {
        var client = GetAuthClient();
        var faker = new Faker();
        var email = faker.Internet.Email().ToLower();
        var password = faker.Internet.Password();

        var address = new Address()
        {
            Street = faker.Address.StreetName(),
            Apartment = faker.Address.BuildingNumber(),
            City = faker.Address.City(),
            Country = faker.Address.Country(),
            State = faker.Address.State(),
            ZipCode = faker.Address.ZipCode()
        };

        var customData = new CustomUserMetadata()
        {
            Address = address
        };

        await client.SignUpAsync(email, password, customData);
        var authenticateUser = await client.SignInAsync<CustomUserMetadata>(email, password);
        var user = authenticateUser.User;
        user.UserMetadata.Address.Should().NotBeNull();
        user.UserMetadata.Address.Should().BeEquivalentTo(address);
        user.UserMetadata.Should().NotBeNull();
        user.UserMetadata.Email.Should().Be(email);
        user.UserMetadata.EmailVerified.Should().BeFalse();
        user.UserMetadata.PhoneVerified.Should().BeFalse();
        user.UserMetadata.Sub.Should().NotBeNullOrEmpty();

        var identity = user.Identities[0];
        identity.IdentityData.Address.Should().NotBeNull();
        identity.IdentityData.Address.Should().BeEquivalentTo(address);
    }

    [Fact]
    public async Task SignInWithPhoneAsync_WithCustomData_ReturnsAccessTokenWithUser()
    {
        var client = GetAuthClient();
        var faker = new Faker();
        var phone = faker.Phone.PhoneNumber("55##########");
        var password = faker.Internet.Password();

        var address = new Address()
        {
            Street = faker.Address.StreetName(),
            Apartment = faker.Address.BuildingNumber(),
            City = faker.Address.City(),
            Country = faker.Address.Country(),
            State = faker.Address.State(),
            ZipCode = faker.Address.ZipCode()
        };

        var customData = new CustomUserMetadata()
        {
            Address = address
        };

        await client.SignUpWithPhoneAsync(phone, password, customData);
        var authenticatedUser = await client.SignInWithPhoneAsync<CustomUserMetadata>(phone, password);
        var user = authenticatedUser.User;
        user.UserMetadata.Address.Should().NotBeNull();
        user.UserMetadata.Address.Should().BeEquivalentTo(address);
        user.UserMetadata.Should().NotBeNull();
        user.UserMetadata.Email.Should().BeNullOrEmpty();
        user.UserMetadata.Sub.Should().NotBeNullOrEmpty();

        var identity = user.Identities[0];
        identity.IdentityData.Address.Should().NotBeNull();
        identity.IdentityData.Address.Should().BeEquivalentTo(address);
    }

    [Fact]
    public async Task SignInWithPhoneAsync_ValidParameters_ReturnsAccessToken()
    {
        var client = GetAuthClient();
        var faker = new Faker();
        var phone = faker.Phone.PhoneNumber("55##########");
        var password = faker.Internet.Password();

        await client.SignUpWithPhoneAsync(phone, password);

        var accessToken = await client.SignInWithPhoneAsync(phone, password);

        accessToken.Should().NotBeNull();

        accessToken.AccessToken.Should().NotBeNullOrEmpty();
        accessToken.TokenType.Should().NotBeNullOrEmpty();
        accessToken.ExpiresIn.Should().BeGreaterThan(0);
        accessToken.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SignInAsync_EmptyPassword_ThrowsArgumentNullException()
    {
        var client = GetAuthClient();
        var faker = new Faker();
        var email = faker.Internet.Email().ToLower();

        var action = async () => await client.SignInAsync(email, string.Empty);

        var ex = await action.Should().ThrowAsync<ArgumentNullException>();
        ex.WithMessage("Password cannot be null or empty. (Parameter 'Password')");
    }

    [Fact]
    public async Task SignInAsync_EmptyEmail_ThrowsArgumentNullException()
    {
        var client = GetAuthClient();
        var faker = new Faker();
        var password = faker.Internet.Password();

        var action = async () => await client.SignInAsync(string.Empty, password);

        var ex = await action.Should().ThrowAsync<ArgumentNullException>();
        ex.WithMessage("E-mail or Phone cannot be null or empty.");
    }

    [Fact]
    public async Task SignInWithPhoneAsync_EmptyPhone_ThrowsArgumentNullException()
    {
        var client = GetAuthClient();
        var faker = new Faker();
        var password = faker.Internet.Password();

        var action = async () => await client.SignInWithPhoneAsync(string.Empty, password);

        var ex = await action.Should().ThrowAsync<ArgumentNullException>();
        ex.WithMessage("E-mail or Phone cannot be null or empty.");
    }

    [Fact]
    public async Task SignInWithPhoneAsync_EmptyPassword_ThrowsArgumentNullException()
    {
        var client = GetAuthClient();
        var faker = new Faker();
        var phone = faker.Phone.PhoneNumber("55##########");

        var action = async () => await client.SignInWithPhoneAsync(phone, string.Empty);

        var ex = await action.Should().ThrowAsync<ArgumentNullException>();
        ex.WithMessage("Password cannot be null or empty. (Parameter 'Password')");
    }

    [Fact]
    public async Task SignInAsync_InvalidPassword_ThrowsSupabaseException()
    {
        var client = GetAuthClient();
        var faker = new Faker();
        var email = faker.Internet.Email().ToLower();
        var password = faker.Internet.Password(3);

        var action = async () => await client.SignInAsync(email, password);
        await action.Should().ThrowAsync<SupabaseException>();
    }

    [Fact]
    public async Task GetSettingsAsync_ReturnsSettings()
    {
        var client = GetAuthClient();
        var settings = await client.GetSettingsAsync();

        settings.Should().NotBeNull();
        settings.DisableSignup.Should().BeFalse();
        settings.AutoConfirm.Should().BeFalse();
        settings.External.Should().NotBeNull();
        settings.External.Apple.Should().BeFalse();
        settings.External.Azure.Should().BeFalse();
        settings.External.Bitbucket.Should().BeFalse();
        settings.External.Discord.Should().BeFalse();
        settings.External.Facebook.Should().BeFalse();
        settings.External.Figma.Should().BeFalse();
        settings.External.Github.Should().BeFalse();
        settings.External.Gitlab.Should().BeFalse();
        settings.External.Google.Should().BeFalse();
        settings.External.Keycloak.Should().BeFalse();
        settings.External.Linkedin.Should().BeFalse();
        settings.External.Notion.Should().BeFalse();
        settings.External.Slack.Should().BeFalse();
        settings.External.Spotify.Should().BeFalse();
        settings.External.Twitch.Should().BeFalse();
        settings.External.Twitter.Should().BeFalse();
        settings.External.Workos.Should().BeFalse();
    }

    [Fact]
    public async Task CreateUserAsync_ValidRequest_ReturnUserCreated()
    {
        var client = GetAuthClient(await CreateResolverForAdminAsync());
        var faker = new Faker();

        var request = new CreateUserRequest()
        {
            Email = faker.Internet.Email().ToLower(),
            Password = faker.Internet.Password(),
            Phone = faker.Phone.PhoneNumber("55##########"),
            Role = faker.Database.Random.String(),
            EmailConfirm = false,
            PhoneConfirm = false
        };

        var user = await client.CreateUserAsync(request);

        user.Id.Should().NotBeEmpty();
        user.Aud.Should().NotBeNullOrEmpty();
        user.Role.Should().NotBeNullOrEmpty();

        user.Email.Should().Be(request.Email);
        user.Phone.Should().Be(request.Phone);

        user.PhoneConfirmedAt.Should().BeNull();

        user.AppMetadata.Should().NotBeNull();
        user.AppMetadata.Provider.Should().Be("email");
        user.AppMetadata.Providers.Should().Contain("email");
        user.AppMetadata.Providers.Should().Contain("phone");

        user.Identities.Should().NotBeNull();

        foreach (var identity in user.Identities)
        {
            if (identity.Provider == "email")
            {
                identity.Email.Should().Be(request.Email);
                identity.IdentityData.Email.Should().Be(request.Email);
            }
            else
            {
                identity.Provider.Should().Be("phone");
                identity.IdentityData.Phone.Should().Be(request.Phone);
            }

            identity.Id.Should().NotBeEmpty();
            identity.IdentityId.Should().NotBeEmpty();
            identity.UserId.Should().NotBeEmpty();
            identity.IdentityData.Should().NotBeNull();
            identity.CreatedAt.Should().NotBe(default);
            identity.UpdatedAt.Should().NotBe(default);
        }
    }

    [Fact]
    public async Task CreateUserAsync_WithCustomData_ReturnUserCreated()
    {
        var client = GetAuthClient(await CreateResolverForAdminAsync());
        var faker = new Faker();

        var address = new Address()
        {
            Street = faker.Address.StreetName(),
            Apartment = faker.Address.BuildingNumber(),
            City = faker.Address.City(),
            Country = faker.Address.Country(),
            State = faker.Address.State(),
            ZipCode = faker.Address.ZipCode()
        };

        var request = new CreateUserRequest<CustomUserMetadata>()
        {
            Email = faker.Internet.Email().ToLower(),
            Password = faker.Internet.Password(),
            Phone = faker.Phone.PhoneNumber("55##########"),
            Role = faker.Database.Random.String(),
            EmailConfirm = false,
            PhoneConfirm = false,
            UserMetadata = new CustomUserMetadata()
            {
                Address = address
            }
        };

        var user = await client.CreateUserAsync(request);
        user.UserMetadata.Should().NotBeNull();
        user.UserMetadata.Address.Should().BeEquivalentTo(address);
    }

    [Fact]
    public async Task UpdateUserAsync_TypedMetadata_UpdatedUser()
    {
        var client = GetAuthClient(await CreateResolverForAdminAsync());
        var faker = new Faker();

        var address = new Address()
        {
            Street = faker.Address.StreetName(),
            Apartment = faker.Address.BuildingNumber(),
            City = faker.Address.City(),
            Country = faker.Address.Country(),
            State = faker.Address.State(),
            ZipCode = faker.Address.ZipCode()
        };

        var request = new CreateUserRequest<CustomUserMetadata>()
        {
            Email = faker.Internet.Email().ToLower(),
            Password = faker.Internet.Password(),
            Phone = faker.Phone.PhoneNumber("55##########"),
            Role = faker.Database.Random.String(),
            EmailConfirm = false,
            PhoneConfirm = false,
            UserMetadata = new CustomUserMetadata()
            {
                Address = address
            }
        };

        var user = await client.CreateUserAsync(request);

        user.UserMetadata.Address = new Address()
        {
            Street = faker.Address.StreetName(),
            Apartment = faker.Address.BuildingNumber(),
            City = faker.Address.City(),
            Country = faker.Address.Country(),
            State = faker.Address.State(),
            ZipCode = faker.Address.ZipCode()
        };

        var updatedUser = await client.UpdateUserAsAdminAsync(user);

        updatedUser.UserMetadata.Address.Should().BeEquivalentTo(user.UserMetadata.Address);
    }

    [Fact]
    public async Task UpdateUserAsync_NoTypedMetadata_UpdatedUser()
    {
        var client = GetAuthClient(await CreateResolverForAdminAsync());
        var faker = new Faker();

        var address = new Address()
        {
            Street = faker.Address.StreetName(),
            Apartment = faker.Address.BuildingNumber(),
            City = faker.Address.City(),
            Country = faker.Address.Country(),
            State = faker.Address.State(),
            ZipCode = faker.Address.ZipCode()
        };

        var request = new CreateUserRequest<CustomUserMetadata>()
        {
            Email = faker.Internet.Email().ToLower(),
            Password = faker.Internet.Password(),
            Phone = faker.Phone.PhoneNumber("55##########"),
            Role = faker.Database.Random.String(),
            EmailConfirm = false,
            PhoneConfirm = false,
            UserMetadata = new CustomUserMetadata()
            {
                Address = address
            }
        };

        var user = await client.CreateUserAsync(request);

        user.UserMetadata.Address = new Address()
        {
            Street = faker.Address.StreetName(),
            Apartment = faker.Address.BuildingNumber(),
            City = faker.Address.City(),
            Country = faker.Address.Country(),
            State = faker.Address.State(),
            ZipCode = faker.Address.ZipCode()
        };

        var updateObj = new
        {
            user_metadata = new
            {
                address = new
                {
                    city = faker.Address.City()
                }
            }
        };

        var updatedUser = await client.UpdateUserAsAdminAsync<CustomUserMetadata>(user.Id, updateObj);
        updatedUser.UserMetadata.Address.City.Should().Be(updateObj.user_metadata.address.city);
    }

    [Fact]
    public async Task GenerateLinkAsync_MagicLink_GeneratedLink()
    {
        var client = GetAuthClient(await CreateResolverForAdminAsync());
        var faker = new Faker();

        var request = new GenerateLinkRequest<CustomUserMetadata>()
        {
            Email = faker.Internet.Email(),
            Type = ActionType.MagicLink,
            Data = new CustomUserMetadata()
            {
                Address = new Address()
                {
                    Street = faker.Address.StreetName(),
                    Apartment = faker.Address.BuildingNumber(),
                    City = faker.Address.City(),
                    Country = faker.Address.Country(),
                    State = faker.Address.State(),
                    ZipCode = faker.Address.ZipCode()
                }
            }
        };

        var link = await client.GenerateLinkAsync(request);

        link.Should().NotBeNull();
        link.UserMetadata.Should().BeEquivalentTo(request.Data);
        link.RedirectTo.Should().NotBeNullOrEmpty();
        link.HashedToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task InviteAsync_ValidEmail_InvitedInfo()
    {
        var client = GetAuthClient(await CreateResolverForAdminAsync());
        var faker = new Faker();
        var email = faker.Internet.Email().ToLower();

        var response = await client.InviteAsync(email);

        response.Should().NotBeNull();
        response.Email.Should().Be(email);
        response.InvitedAt.Should().NotBe(default);
        response.ConfirmationSentAt.Should().NotBe(default);
        response.CreatedAt.Should().NotBe(default);
        response.UpdatedAt.Should().NotBe(default);
    }

    [Fact]
    public async Task RecoverAsync_ValidEmail_SuccessRecover()
    {
        var client = GetAuthClient();
        var faker = new Faker();
        var email = faker.Internet.Email().ToLower();
        var password = faker.Internet.Password().ToLower();

        await client.SignUpAsync(email, password);

        await client.RecoverAsync(email);
    }

    [Fact]
    public async Task GetUserAsync_ValidId_ReturnsUser()
    {
        var client = GetAuthClient(await CreateResolverForAdminAsync());
        var faker = new Faker();
        var email = faker.Internet.Email().ToLower();
        var password = faker.Internet.Password();

        var createdUser = await client.SignUpAsync(email, password);

        var user = await client.GetUserAsync(createdUser.User.Id);

        user.Should().NotBeNull();
        user.Id.Should().Be(createdUser.User.Id);
    }

    [Fact]
    public async Task GetUserAsync_WithCustomData_ReturnsUser()
    {
        var client = GetAuthClient(await CreateResolverForAdminAsync());
        var faker = new Faker();
        var email = faker.Internet.Email().ToLower();
        var password = faker.Internet.Password();

        var address = new Address()
        {
            Street = faker.Address.StreetName(),
            Apartment = faker.Address.BuildingNumber(),
            City = faker.Address.City(),
            Country = faker.Address.Country(),
            State = faker.Address.State(),
            ZipCode = faker.Address.ZipCode()
        };

        var customData = new CustomUserMetadata()
        {
            Address = address
        };

        var createdUser = await client.SignUpAsync(email, password, customData);

        var user = await client.GetUserAsync<CustomUserMetadata>(createdUser.User.Id);

        user.UserMetadata.Address.Should().NotBeNull();
        user.UserMetadata.Address.Should().BeEquivalentTo(address);
    }

    //Todo: Need a better way to test this.
    // [Fact]
    // public async Task GetCurrentUserAsync_ReturnsUser()
    // {
    //     var client = GetAuthClient();
    //     var faker = new Faker();
    //
    //     var user = await client.GetCurrentUserAsync();
    //
    //     user.Should().NotBeNull();
    // }

    private async ValueTask<TokenResolver> CreateResolverForAdminAsync()
    {
        var token = await GetAdminTokenAsync();
        var resolver = new TokenResolver
        {
            GetTokenDel = () => token
        };
        return resolver;
    }

    private async ValueTask<string> GetAdminTokenAsync()
    {
        return Environment.GetEnvironmentVariable("SUPABASE_SERVICE_ROLE");
    }

    private ISupabaseAuth GetAuthClient(TokenResolver? resolver = null)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSupabase("http://localhost:54321", "test-api-key-generated");

        if (resolver != null)
            serviceCollection.AddScoped<ITokenResolver, TokenResolver>(_ => resolver);

        var provider = serviceCollection.BuildServiceProvider();
        var client = provider.GetRequiredService<ISupabaseAuth>();

        return client;
    }
}