namespace ExampleWebApp.Backend.WebApi;

public enum DeleteUserStatus
{
    /// <summary>
    /// Delete user ok.
    /// </summary>    
    OK,

    /// <summary>
    /// asp net core IdentityError, see Errors for details.
    /// </summary>    
    IdentityError,

    /// <summary>
    /// User not found.
    /// </summary>    
    UserNotFound,    

    /// <summary>
    /// Cannot delete last non disabled admin user.
    /// </summary>
    CannotDeleteLastActiveAdmin,

    PermissionsError

}

public class DeleteUserResponseDto
{

    /// <summary>
    /// API specific status response.
    /// </summary>    
    public required DeleteUserStatus Status { get; set; }

    /// <summary>
    /// List of edit user errors if any.
    /// </summary>    
    public List<string> Errors { get; set; } = new();

}
