using Refit;
using Supabase.Authentication.Auth.GoTrue.Requests;
using Supabase.Authentication.Auth.GoTrue.Responses;
using Supabase.Common;
using Supabase.Utils.Attributes;

namespace Supabase.Authentication.Auth.GoTrue;

internal interface IGoTrueApi
{
    [Post("/token")]
    Task<SignInResponse<TCustomMetadata>> TokenAsync<TRequest, TCustomMetadata>([Body] TRequest request,
        [AliasAs("grant_type")] [Query] string grantType,
        CancellationToken cancellationToken)
        where TRequest : class
        where TCustomMetadata : UserMetadataBase;

    [Post("/logout")]
    Task LogoutAsync(CancellationToken cancellationToken);

    [RequiresAdmin]
    [Post("/signup")]
    Task<SignInResponse<TCustomMetadata>> SignUpAsync<TCustomMetadata>([Body] SignUpRequest request,
        CancellationToken cancellationToken) where TCustomMetadata : UserMetadataBase;

    [RequiresAdmin]
    [Post("/signup")]
    Task<SignInResponse<TCustomMetadata>> SignUpAsync<TCustomMetadata>(CancellationToken cancellationToken)
        where TCustomMetadata : UserMetadataBase;

    [RequiresAdmin]
    [Get("/settings")]
    Task<SettingsResponse> SettingsAsync(CancellationToken cancellationToken);

    [RequiresAdmin]
    [Post("/admin/users")]
    Task<UserResponse<TCustomMetadata>> CreateUserAsync<TCustomMetadata>(
        CreateUserRequest<TCustomMetadata> request,
        CancellationToken cancellationToken) where TCustomMetadata : UserMetadataBase;

    [RequiresAdmin]
    [Get("/admin/users/{userId}")]
    Task<UserResponse<TCustomMetadata>> GetUserAsync<TCustomMetadata>(Guid userId,
        CancellationToken cancellationToken) where TCustomMetadata : UserMetadataBase;

    [RequiresAdmin]
    [Put("/admin/users/{userId}")]
    Task<UserResponse<TCustomMetadata>> UpdateUserAsAdminAsync<TCustomMetadata>([Body] object request,
        Guid userId,
        CancellationToken cancellationToken) where TCustomMetadata : UserMetadataBase;

    [Put("/user")]
    Task<UserResponse<TCustomMetadata>> UpdateUserAsync<TCustomMetadata>([Body] object request,
        CancellationToken cancellationToken) where TCustomMetadata : UserMetadataBase;

    [RequiresAdmin]
    [Post("/admin/generate_link")]
    Task<GenerateLinkResponse<TCustomMetadata>> GenerateLinkAsync<TCustomMetadata>(
        [Body] GenerateLinkRequest<TCustomMetadata> request,
        CancellationToken cancellationToken) where TCustomMetadata : UserMetadataBase;

    [RequiresAdmin]
    [Post("/invite")]
    Task<InviteResponse> InviteAsync(InviteRequest request, CancellationToken cancellationToken);

    [RequiresAdmin]
    [Post("/recover")]
    Task<InviteResponse> RecoverAsync(RecoverRequest request, CancellationToken cancellationToken);

    [Get("/user")]
    Task<UserResponse<TCustomMetadata>> GetCurrentUserAsync<TCustomMetadata>(CancellationToken cancellationToken)
        where TCustomMetadata : UserMetadataBase;

    [Put("/user")]
    Task<UserResponse<TCustomMetadata>> UpdateCurrentUserAsync<TCustomMetadata>([Body] object request,
        CancellationToken cancellationToken) where TCustomMetadata : UserMetadataBase;

    [Post("/otp")]
    Task<SignInWithOtpResponse> SignInWithOtpAsync([Body] SignInWithOtpRequest request,
        CancellationToken cancellationToken);

    [Post("/verify")]
    Task<SignInResponse<TCustomMetadata>> VerifyOtpAsync<TCustomMetadata>([Body] VerifyOtpRequest request,
        CancellationToken cancellationToken) where TCustomMetadata : UserMetadataBase;
}