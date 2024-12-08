using Refit;
using Supabase.Authentication.Auth.GoTrue.Requests;
using Supabase.Authentication.Auth.GoTrue.Responses;
using Supabase.Common;

namespace Supabase.Authentication.Auth.GoTrue;

public interface IGoTrueApi
{
    [Post("/token")]
    Task<SignInResponse<TCustomMetadata>> TokenAsync<TCustomMetadata>([Body] SignInRequest request,
        [AliasAs("grant_type")] [Query] string grantType,
        CancellationToken cancellationToken) where TCustomMetadata : UserMetadataBase;

    [Post("/logout")]
    Task<SignInResponse<TCustomMetadata>> LogoutAsync<TCustomMetadata>(CancellationToken cancellationToken)
        where TCustomMetadata : UserMetadataBase;

    [Post("/signup")]
    Task<SignInResponse<TCustomMetadata>> SignUpAsync<TCustomMetadata>([Body] SignUpRequest request,
        CancellationToken cancellationToken) where TCustomMetadata : UserMetadataBase;

    [Post("/signup")]
    Task<SignInResponse<TCustomMetadata>> SignUpAsync<TCustomMetadata>(CancellationToken cancellationToken)
        where TCustomMetadata : UserMetadataBase;

    [Get("/settings")]
    Task<SettingsResponse> SettingsAsync(CancellationToken cancellationToken);

    [Post("/admin/users")]
    Task<UserResponse<TCustomMetadata>> CreateUserAsync<TCustomMetadata>(
        CreateUserRequest<TCustomMetadata> request,
        CancellationToken cancellationToken) where TCustomMetadata : UserMetadataBase;

    [Get("/admin/users/{userId}")]
    Task<UserResponse<TCustomMetadata>> GetUserAsync<TCustomMetadata>(Guid userId,
        CancellationToken cancellationToken) where TCustomMetadata : UserMetadataBase;

    [Put("/admin/users/{userId}")]
    Task<UserResponse<TCustomMetadata>> UpdateUserAsAdminAsync<TCustomMetadata>([Body] object request,
        Guid userId,
        CancellationToken cancellationToken) where TCustomMetadata : UserMetadataBase;

    [Put("/users")]
    Task<UserResponse<TCustomMetadata>> UpdateUserAsync<TCustomMetadata>([Body] object request,
        CancellationToken cancellationToken) where TCustomMetadata : UserMetadataBase;

    [Post("/admin/generate_link")]
    Task<GenerateLinkResponse<TCustomMetadata>> GenerateLinkAsync<TCustomMetadata>(
        [Body] GenerateLinkRequest<TCustomMetadata> request,
        CancellationToken cancellationToken) where TCustomMetadata : UserMetadataBase;

    [Post("/invite")]
    Task<InviteResponse> InviteAsync(InviteRequest request, CancellationToken cancellationToken);

    [Post("/recover")]
    Task<InviteResponse> RecoverAsync(RecoverRequest request, CancellationToken cancellationToken);

    [Get("/user")]
    Task<UserResponse<TCustomMetadata>> GetCurrentUserAsync<TCustomMetadata>(CancellationToken cancellationToken)
        where TCustomMetadata : UserMetadataBase;

    [Put("/user")]
    Task<UserResponse<TCustomMetadata>> UpdateCurrentUserAsync<TCustomMetadata>([Body] object request,
        CancellationToken cancellationToken) where TCustomMetadata : UserMetadataBase;
}