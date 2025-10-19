namespace ExampleWebApp.Backend.WebApi.Services.Abstractions.Auth;

/// <summary>
/// Authentication service.
/// </summary>
public interface IAuthService
{

    /// <summary>
    /// Get auth options required for username and password.
    /// </summary>
    AuthOptions AuthOptions();

    /// <summary>
    /// Login user by given username or email and auth password.
    /// </summary>
    /// <param name="loginRequestDto">User login info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Login response and JWT if successfully logged in.</returns>
    Task<LoginResponseDto> LoginAsync(
        LoginRequestDto loginRequestDto, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieve current logged in user name, email, roles.
    /// </summary>        
    /// <returns>Logged in user info or null if not authenticated.</returns>                
    CurrentUserResponseDto CurrentUserNfo();

    /// <summary>
    /// Renew access token if valid refresh token was found
    /// </summary>
    Task<RenewAccessTokenResponse> RenewCurrentUserAccessTokenAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Renew refresh token of current user if refresh token still valid.
    /// This is used to extends refresh token duration avoiding closing frontend session.
    /// </summary>
    Task<RenewRefreshTokenResponse> RenewCurrentUserRefreshTokenAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Logout current user.
    /// </summary>    
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<HttpStatusCode> LogoutAsync(CancellationToken cancellationToken);

    /// <summary>
    /// List all users.
    /// </summary>    
    /// <param name="username">If not null list only given user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of users</returns>
    Task<List<UserListItemResponseDto>> ListUsersAsync(
        CancellationToken cancellationToken,
        string? username = null);

    /// <summary>
    /// Delete given user.
    /// </summary>
    Task<DeleteUserResponseDto> DeleteUserAsync(
        string usernameToDelete, CancellationToken cancellationToken);

    /// <summary>
    /// List all roles.
    /// </summary>    
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of users roles</returns>
    Task<List<string>> ListRolesAsync(
        CancellationToken cancellationToken);

    /// <summary>
    /// Create or edit given user.
    /// Privileges are managed by the authenticated user roles ( <see cref="ROLE_admin"/>, <see cref="ROLE_advanced"/>, <see cref="ROLE_normal"/>    ).
    /// </summary>
    /// <param name="editUserRequestDto">Edit user request data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>    
    Task<EditUserResponseDto> EditUserAsync(
        EditUserRequestDto editUserRequestDto, CancellationToken cancellationToken);

    Task<ResetLostPasswordResponseDto> ResetLostPasswordRequestAsync(
        string email, string? token, string? resetPassword, CancellationToken cancellationToken);

}
