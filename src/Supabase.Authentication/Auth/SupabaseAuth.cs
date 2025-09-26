using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Supabase.Authentication.Auth.GoTrue;
using Supabase.Authentication.Auth.GoTrue.Requests;
using Supabase.Authentication.Auth.GoTrue.Responses;
using Supabase.Clients;
using Supabase.Common;
using Supabase.Common.TokenResolver;
using Supabase.Utils.Extensions;

namespace Supabase.Authentication.Auth;

/// <summary>
/// Client for authentication in Supabase.
/// </summary>
internal class SupabaseAuth : ClientBase<SupabaseAuth>, ISupabaseAuth
{
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly ITokenResolver _tokenResolver;
    private readonly IGoTrueApi _goTrueApi;
    public SupabaseAuth(ILogger<SupabaseAuth> logger, IGoTrueApi goTrueApi, ITokenResolver tokenResolver,
        IOptionsMonitor<JwtBearerOptions> jwtOptions) : base(logger)
    {
        _tokenValidationParameters = jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme).TokenValidationParameters;
        _goTrueApi = goTrueApi;
        _tokenResolver = tokenResolver;
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    private async ValueTask<SignInResponse<TCustomMetadata>> SignInAsync<TCustomMetadata>(SignInRequest request,
        CancellationToken cancellationToken = default) where TCustomMetadata : UserMetadataBase
    {
        if (request.Password.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(request.Password), "Password cannot be null or empty.");

        if (request.Email.IsNullOrEmpty() && request.Phone.IsNullOrEmpty())
            throw new ArgumentNullException(null, "E-mail or Phone cannot be null or empty.");

        if (!request.Email.IsNullOrEmpty() && !request.Phone.IsNullOrEmpty())
            throw new ArgumentException("E-mail and Phone cannot be filled at the same time.");

        var response =
            await _goTrueApi.TokenAsync<SignInRequest, TCustomMetadata>(request, "password", cancellationToken);
        return response;
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    private async ValueTask<SignInResponse<TCustomMetadata>> SignUpAsync<TCustomMetadata>(SignUpRequest request,
        CancellationToken cancellationToken = default) where TCustomMetadata : UserMetadataBase
    {
        if (request.Password.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(request.Password), "Password cannot be null or empty.");

        if (request.Email.IsNullOrEmpty() && request.Phone.IsNullOrEmpty())
            throw new ArgumentNullException(null, message: "E-mail or Phone cannot be null or empty.");

        if (!request.Email.IsNullOrEmpty() && !request.Phone.IsNullOrEmpty())
            throw new ArgumentException("E-mail and Phone cannot be filled at the same time.");

        return await _goTrueApi.SignUpAsync<TCustomMetadata>(request, cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public ValueTask<SignInResponse<UserMetadataBase>> SignInAsync(string email, string password,
        CancellationToken cancellationToken = default) =>
        SignInAsync<UserMetadataBase>(email, password, cancellationToken: cancellationToken);

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<SignInResponse<TCustomMetadata>> SignInAsync<TCustomMetadata>(string email, string password,
        CancellationToken cancellationToken = default) where TCustomMetadata : UserMetadataBase
    {
        var request = new SignInRequest()
        {
            Email = email,
            Password = password
        };

        return await SignInAsync<TCustomMetadata>(request, cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public ValueTask<SignInResponse<UserMetadataBase>> SignInWithPhoneAsync(string phone, string password,
        CancellationToken cancellationToken = default) =>
        SignInWithPhoneAsync<UserMetadataBase>(phone, password, cancellationToken);

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<SignInResponse<TCustomMetadata>> SignInWithPhoneAsync<TCustomMetadata>(string phone,
        string password,
        CancellationToken cancellationToken = default) where TCustomMetadata : UserMetadataBase
    {
        var request = new SignInRequest()
        {
            Phone = phone,
            Password = password
        };

        return await SignInAsync<TCustomMetadata>(request, cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask LogoutAsync(CancellationToken cancellationToken = default) =>
        await _goTrueApi.LogoutAsync(cancellationToken);

    /// <inheritdoc cref="ISupabaseAuth"/>
    public ValueTask<SignInResponse<UserMetadataBase>> SignUpAsync(string email, string password,
        CancellationToken cancellationToken = default)
        => SignUpAsync<UserMetadataBase>(email, password, cancellationToken: cancellationToken);

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<SignInResponse<TCustomMetadata>> SignUpAsync<TCustomMetadata>(string email, string password,
        TCustomMetadata? data = null,
        CancellationToken cancellationToken = default) where TCustomMetadata : UserMetadataBase
    {
        var request = new SignUpRequest()
        {
            Email = email,
            Password = password,
            Data = data
        };

        return await SignUpAsync<TCustomMetadata>(request, cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public ValueTask<SignInResponse<UserMetadataBase>> SignUpWithPhoneAsync(string phone, string password,
        CancellationToken cancellationToken = default) =>
        SignUpWithPhoneAsync<UserMetadataBase>(phone, password, cancellationToken: cancellationToken);

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<SignInResponse<TCustomMetadata>> SignUpWithPhoneAsync<TCustomMetadata>(string phone,
        string password,
        TCustomMetadata? data = null,
        CancellationToken cancellationToken = default) where TCustomMetadata : UserMetadataBase
    {
        var request = new SignUpRequest()
        {
            Phone = phone,
            Password = password,
            Data = data
        };

        return await SignUpAsync<TCustomMetadata>(request, cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<SettingsResponse> GetSettingsAsync(CancellationToken cancellationToken = default)
    {
        return await _goTrueApi.SettingsAsync(cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public ValueTask<UserResponse<UserMetadataBase>> CreateUserAsync(
        CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var realRequest = request as CreateUserRequest<UserMetadataBase>;
        return CreateUserAsync(realRequest, cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<UserResponse<TCustomMetadata>> CreateUserAsync<TCustomMetadata>(
        CreateUserRequest<TCustomMetadata> request, CancellationToken cancellationToken = default)
        where TCustomMetadata : UserMetadataBase
    {
        return await _goTrueApi.CreateUserAsync(request, cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public ValueTask<UserResponse<UserMetadataBase>> UpdateUserAsync(UserResponse<UserMetadataBase> user,
        CancellationToken cancellationToken = default) => UpdateUserAsync<UserMetadataBase>(user, cancellationToken);

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<UserResponse<TCustomMetadata>> UpdateUserAsync<TCustomMetadata>(
        UserResponse<TCustomMetadata> user,
        CancellationToken cancellationToken = default) where TCustomMetadata : UserMetadataBase
    {
        //Necessary to prevent admin privileges requirement.
        user.AppMetadata = null!;
        user.Identities = null!;

        return await _goTrueApi.UpdateUserAsync<TCustomMetadata>(user, cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public ValueTask<SignInResponse<UserMetadataBase>> RefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default) =>
        RefreshTokenAsync<UserMetadataBase>(refreshToken, cancellationToken);

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<SignInResponse<TCustomMetadata>> RefreshTokenAsync<TCustomMetadata>(string refreshToken,
        CancellationToken cancellationToken = default) where TCustomMetadata : UserMetadataBase
    {
        var request = new RefreshTokenRequest(refreshToken);
        return await RefreshTokenAsync<TCustomMetadata>(request, cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    private async ValueTask<SignInResponse<TCustomMetadata>> RefreshTokenAsync<TCustomMetadata>(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default) where TCustomMetadata : UserMetadataBase
    {
        if (request.RefreshToken.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(request.RefreshToken), "Refresh token cannot be null or empty.");

        var response =
            await _goTrueApi.TokenAsync<RefreshTokenRequest, TCustomMetadata>(request, "refresh_token",
                cancellationToken);
        return response;
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public ValueTask<UserResponse<UserMetadataBase>> UpdateUserAsync(object user,
        CancellationToken cancellationToken = default) =>
        UpdateUserAsync<UserMetadataBase>(user, cancellationToken);

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<UserResponse<TCustomMetadata>> UpdateUserAsync<TCustomMetadata>(object user,
        CancellationToken cancellationToken = default)
        where TCustomMetadata : UserMetadataBase
    {
        return await _goTrueApi.UpdateUserAsync<TCustomMetadata>(user, cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public ValueTask<UserResponse<UserMetadataBase>> UpdateUserAsAdminAsync(UserResponse<UserMetadataBase> user,
        CancellationToken cancellationToken = default) =>
        UpdateUserAsAdminAsync<UserMetadataBase>(user, cancellationToken);

    /// <inheritdoc cref="ISupabaseAuth"/>
    public ValueTask<UserResponse<TCustomMetadata>> UpdateUserAsAdminAsync<TCustomMetadata>(
        UserResponse<TCustomMetadata> user, CancellationToken cancellationToken = default)
        where TCustomMetadata : UserMetadataBase =>
        UpdateUserAsAdminAsync<TCustomMetadata>(user.Id, user, cancellationToken);

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<UserResponse<TCustomMetadata>> UpdateUserAsAdminAsync<TCustomMetadata>(Guid userId,
        object request, CancellationToken cancellationToken = default)
        where TCustomMetadata : UserMetadataBase
    {
        return await _goTrueApi.UpdateUserAsAdminAsync<TCustomMetadata>(request, userId, cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public ValueTask<UserResponse<UserMetadataBase>> UpdateUserAsAdminAsync(Guid userId,
        object request, CancellationToken cancellationToken = default) =>
        UpdateUserAsAdminAsync<UserMetadataBase>(userId, request, cancellationToken);

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<GenerateLinkResponse<TCustomMetadata>> GenerateLinkAsync<TCustomMetadata>(
        GenerateLinkRequest<TCustomMetadata> request,
        CancellationToken cancellationToken = default) where TCustomMetadata : UserMetadataBase
    {
        return await _goTrueApi.GenerateLinkAsync(request, cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public ValueTask<GenerateLinkResponse<UserMetadataBase>> GenerateLinkAsync(
        GenerateLinkRequest<UserMetadataBase> request,
        CancellationToken cancellationToken = default) =>
        GenerateLinkAsync<UserMetadataBase>(request, cancellationToken);

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<InviteResponse> InviteAsync(string email, object? data = null,
        CancellationToken cancellationToken = default)
    {
        var request = new InviteRequest(email, data);
        return await _goTrueApi.InviteAsync(request, cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask RecoverAsync(string email, CancellationToken cancellationToken = default)
    {
        var request = new RecoverRequest(email);
        await _goTrueApi.RecoverAsync(request, cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<UserResponse<TCustomMetadata>> GetCurrentUserAsync<TCustomMetadata>(
        CancellationToken cancellationToken = default)
        where TCustomMetadata : UserMetadataBase
    {
        return await _goTrueApi.GetCurrentUserAsync<TCustomMetadata>(cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<UserResponse<UserMetadataBase>> GetCurrentUserAsync(
        CancellationToken cancellationToken = default)
    {
        return await _goTrueApi.GetCurrentUserAsync<UserMetadataBase>(cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<UserResponse<TCustomMetadata>> UpdateCurrentUserAsync<TCustomMetadata>(object request,
        CancellationToken cancellationToken = default)
        where TCustomMetadata : UserMetadataBase
    {
        return await _goTrueApi.UpdateCurrentUserAsync<TCustomMetadata>(request, cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public ValueTask<UserResponse<UserMetadataBase>> UpdateCurrentUserAsync(object request,
        CancellationToken cancellationToken = default) =>
        UpdateCurrentUserAsync<UserMetadataBase>(request, cancellationToken);


    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<UserResponse<TCustomMetadata>> GetUserAsync<TCustomMetadata>(Guid userId,
        CancellationToken cancellationToken = default)
        where TCustomMetadata : UserMetadataBase
    {
        return await _goTrueApi.GetUserAsync<TCustomMetadata>(userId, cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public ValueTask<UserResponse<UserMetadataBase>> GetUserAsync(Guid userId,
        CancellationToken cancellationToken = default) => GetUserAsync<UserMetadataBase>(userId, cancellationToken);

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<SignInResponse<UserMetadataBase>> SignInAnonymousAsync(CancellationToken cancellationToken)
    {
        return await _goTrueApi.SignUpAsync<UserMetadataBase>(cancellationToken);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public bool IsValidToken(string? role = null)
    {
        var token = _tokenResolver.GetToken();
        return IsValidToken(token, role);
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public bool IsValidToken(string token, string? role)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();            
            tokenHandler.ValidateToken(token, _tokenValidationParameters, out var securityToken);

            if (string.IsNullOrEmpty(role))
                return true;

            var jwtToken = (JwtSecurityToken)securityToken;
            return jwtToken.Claims.Any(x => x.Type == ClaimTypes.Role && x.Value == role);
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public ClaimsPrincipal? GetClaims(string? token = null)
    {
        try
        {
            var tokenToUse = token ?? _tokenResolver.GetToken();

            if (string.IsNullOrEmpty(tokenToUse))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(tokenToUse);

            var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
            return new ClaimsPrincipal(identity);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to extract claims from JWT token");
            return null;
        }
    }

    /// <inheritdoc cref="ISupabaseAuth"/>
    public async ValueTask<ClaimsPrincipal?> GetClaimsAsync(string? token = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenToUse = token ?? _tokenResolver.GetToken();

            if (string.IsNullOrEmpty(tokenToUse))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var result = await tokenHandler.ValidateTokenAsync(tokenToUse, _tokenValidationParameters);
            return result.IsValid ? new ClaimsPrincipal(result.ClaimsIdentity) : null;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to validate claims, falling back to simple extraction");
            return GetClaims(token);
        }
    }
}