using Supabase.Authentication.Auth.GoTrue.Requests;
using Supabase.Authentication.Auth.GoTrue.Responses;
using Supabase.Clients;
using Supabase.Common;

namespace Supabase.Authentication.Auth;

public interface ISupabaseAuth : IClientBase
{
    /// <summary>
    /// Signs in a user using their email and password.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <exception cref="ArgumentNullException">Thrown if the password or email is null or empty.</exception>
    /// <returns>User logged.</returns>
    ValueTask<SignInResponse<UserMetadataBase>> SignInAsync(string email, string password,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs in a user using their phone number and password.
    /// </summary>
    /// <param name="phone">The phone number of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <exception cref="ArgumentNullException">Thrown if the password or phone number is null or empty.</exception>
    /// <returns>User logged.</returns>
    ValueTask<SignInResponse<UserMetadataBase>> SignInWithPhoneAsync(string phone, string password,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs in a user using their email and password, and returns custom metadata.
    /// </summary>
    /// <typeparam name="TCustomMetadata">The type of custom metadata.</typeparam>
    /// <param name="email">The email address of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <exception cref="ArgumentNullException">Thrown if the password or email is null or empty.</exception>
    /// <returns>User logged.</returns>
    ValueTask<SignInResponse<TCustomMetadata>> SignInAsync<TCustomMetadata>(string email, string password,
        CancellationToken cancellationToken = default) where TCustomMetadata : UserMetadataBase;

    /// <summary>
    /// Signs in a user using their phone number and password, and returns custom metadata.
    /// </summary>
    /// <typeparam name="TCustomMetadata">The type of custom metadata.</typeparam>
    /// <param name="phone">The phone number of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <exception cref="ArgumentNullException">Thrown if the password or phone number is null or empty.</exception>
    /// <returns>User logged.</returns>
    ValueTask<SignInResponse<TCustomMetadata>> SignInWithPhoneAsync<TCustomMetadata>(string phone, string password,
        CancellationToken cancellationToken = default) where TCustomMetadata : UserMetadataBase;

    /// <summary>
    /// Signs out the current user.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous sign-out operation.</returns>
    ValueTask LogoutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs up a new user using their email and password.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>User registered.</returns>
    ValueTask<SignInResponse<UserMetadataBase>> SignUpAsync(string email, string password,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs up a new user using their email and password, and returns custom metadata.
    /// </summary>
    /// <typeparam name="TCustomMetadata">The type of custom metadata.</typeparam>
    /// <param name="email">The email address of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <param name="data">The custom metadata to associate with the user.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>User registered.</returns>
    ValueTask<SignInResponse<TCustomMetadata>> SignUpAsync<TCustomMetadata>(string email, string password,
        TCustomMetadata? data = null, CancellationToken cancellationToken = default)
        where TCustomMetadata : UserMetadataBase;

    /// <summary>
    /// Signs up a new user using their phone number and password.
    /// </summary>
    /// <param name="phone">The phone number of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>User registered.</returns>
    ValueTask<SignInResponse<UserMetadataBase>> SignUpWithPhoneAsync(string phone, string password,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs up a new user using their phone number and password, and returns custom metadata.
    /// </summary>
    /// <typeparam name="TCustomMetadata">The type of custom metadata.</typeparam>
    /// <param name="phone">The phone number of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <param name="data">The custom metadata to associate with the user.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>User registered.</returns>
    ValueTask<SignInResponse<TCustomMetadata>> SignUpWithPhoneAsync<TCustomMetadata>(string phone, string password,
        TCustomMetadata? data = null, CancellationToken cancellationToken = default)
        where TCustomMetadata : UserMetadataBase;

    /// <summary>
    /// Retrieves the settings for the current user.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>Settings from Supabase.</returns>
    ValueTask<SettingsResponse> GetSettingsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">The request to create the user.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>User created.</returns>
    ValueTask<UserResponse<UserMetadataBase>> CreateUserAsync(CreateUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user with custom metadata.
    /// </summary>
    /// <typeparam name="TCustomMetadata">The type of custom metadata.</typeparam>
    /// <param name="request">The request to create the user.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>User created.</returns>
    ValueTask<UserResponse<TCustomMetadata>> CreateUserAsync<TCustomMetadata>(
        CreateUserRequest<TCustomMetadata> request, CancellationToken cancellationToken = default)
        where TCustomMetadata : UserMetadataBase;

    /// <summary>
    /// Updates the information for the current user (Requires admin privileges).
    /// </summary>
    /// <param name="user">The user to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>User updated.</returns>
    ValueTask<UserResponse<UserMetadataBase>> UpdateUserAsAdminAsync(UserResponse<UserMetadataBase> user,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the information for the current user with custom metadata (Requires admin privileges).
    /// </summary>
    /// <typeparam name="TCustomMetadata">The type of custom metadata.</typeparam>
    /// <param name="user">The user to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>User updated.</returns>
    ValueTask<UserResponse<TCustomMetadata>> UpdateUserAsAdminAsync<TCustomMetadata>(UserResponse<TCustomMetadata> user,
        CancellationToken cancellationToken = default) where TCustomMetadata : UserMetadataBase;

    /// <summary>
    /// Updates the information for the user with the given ID (Requires admin privileges).
    /// </summary>
    /// <param name="userId">The ID of the user to update.</param>
    /// <param name="request">The request to update the user.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous update user operation.</returns>
    ValueTask<UserResponse<UserMetadataBase>> UpdateUserAsAdminAsync(Guid userId, object request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the information for the user with the given ID (Requires admin privileges).
    /// </summary>
    /// <param name="userId">The ID of the user to update.</param>
    /// <param name="request">The request to update the user.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <typeparam name="TCustomMetadata">The type of custom metadata.</typeparam>
    /// <returns>Updated user.</returns>
    ValueTask<UserResponse<TCustomMetadata>> UpdateUserAsAdminAsync<TCustomMetadata>(Guid userId,
        object request, CancellationToken cancellationToken = default)
        where TCustomMetadata : UserMetadataBase;

    /// <summary>
    /// Generates a link for the current user.
    /// </summary>
    /// <typeparam name="TCustomMetadata">The type of custom metadata.</typeparam>
    /// <param name="request">The request to generate the link.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>Link generated.</returns>
    ValueTask<GenerateLinkResponse<TCustomMetadata>> GenerateLinkAsync<TCustomMetadata>(
        GenerateLinkRequest<TCustomMetadata> request, CancellationToken cancellationToken = default)
        where TCustomMetadata : UserMetadataBase;

    /// <summary>
    /// Generates a link for the current user.
    /// </summary>
    /// <param name="request">The request to generate the link.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>Link generated.</returns>
    ValueTask<GenerateLinkResponse<UserMetadataBase>> GenerateLinkAsync(GenerateLinkRequest<UserMetadataBase> request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Invites a user to join the application.
    /// </summary>
    /// <param name="email">The email address of the user to invite.</param>
    /// <param name="data">(Optional) Custom data to send.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>Invite info.</returns>
    ValueTask<InviteResponse> InviteAsync(string email, object? data = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Recovers a user's password.
    /// </summary>
    /// <param name="email">The email address of the user to recover.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    ValueTask RecoverAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current user's information.
    /// </summary>
    /// <typeparam name="TCustomMetadata">The type of custom metadata.</typeparam>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>Current user updated.</returns>
    ValueTask<UserResponse<TCustomMetadata>> GetCurrentUserAsync<TCustomMetadata>(
        CancellationToken cancellationToken = default) where TCustomMetadata : UserMetadataBase;

    /// <summary>
    /// Retrieves the current user's information.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>Information about current user.</returns>
    ValueTask<UserResponse<UserMetadataBase>> GetCurrentUserAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current user's information.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <typeparam name="TCustomMetadata">The type of custom metadata.</typeparam>
    /// <returns>Information about current user.</returns>
    ValueTask<UserResponse<TCustomMetadata>> GetUserAsync<TCustomMetadata>(Guid userId,
        CancellationToken cancellationToken = default)
        where TCustomMetadata : UserMetadataBase;

    /// <summary>
    /// Retrieves the current user's information.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>Information about current user.</returns>
    ValueTask<UserResponse<UserMetadataBase>> GetUserAsync(Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs in anonymous.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>Anonymous session.</returns>
    ValueTask<SignInResponse<UserMetadataBase>> SignInAnonymousAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Verifies if the current token is valid.
    /// </summary>
    /// <param name="role">Role to validate.</param>
    /// <returns>True if the token is valid, false otherwise.</returns>
    bool IsValidToken(string? role = null);

    /// <summary>
    /// Verifies if the specified token is valid.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <param name="role">Role to validate.</param>
    /// <returns>True if the token is valid, false otherwise.</returns>
    bool IsValidToken(string token, string role);

    /// <summary>
    /// Updates the information for the current user.
    /// </summary>
    /// <param name="user">User info to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>User updated.</returns>
    ValueTask<UserResponse<UserMetadataBase>> UpdateUserAsync(object user,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the information for the current user.
    /// </summary>
    /// <param name="user">User info to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>User updated.</returns>
    ValueTask<UserResponse<UserMetadataBase>> UpdateUserAsync(UserResponse<UserMetadataBase> user,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the information for the current user.
    /// </summary>
    /// <param name="user">User info to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <typeparam name="TCustomMetadata">The type of custom metadata.</typeparam>
    /// <returns>User updated.</returns>
    ValueTask<UserResponse<TCustomMetadata>> UpdateUserAsync<TCustomMetadata>(object user,
        CancellationToken cancellationToken = default)
        where TCustomMetadata : UserMetadataBase;
}