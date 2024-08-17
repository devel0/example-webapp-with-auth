namespace ExampleWebApp.Backend.WebApi;

public enum EditUserStatus
{
    /// <summary>
    /// Registration ok.
    /// </summary>    
    OK = STATUS_OK,   

    /// <summary>
    /// Can't change admin role.
    /// </summary>    
    AdminRolesReadOnly = STATUS_AdminRolesReadOnly,

    IdentityError = STATUS_IdentityError,

    /// <summary>
    /// User not found.
    /// </summary>    
    UserNotFound = STATUS_UserNotFound,

    /// <summary>
    /// Internal error.
    /// </summary>    
    InternalError = STATUS_InternalError,

       
    InvalidPassword = STATUS_InvalidPassword,
}

public class EditUserResponseDto
{

    /// <summary>
    /// API specific status response.
    /// </summary>    
    public required EditUserStatus Status { get; set; }

    /// <summary>
    /// List of edit user errors if any.
    /// </summary>    
    public List<string> Errors { get; set; } = new();

}
