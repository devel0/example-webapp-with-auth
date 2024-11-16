namespace ExampleWebApp.Backend.WebApi;

public enum EditUserStatus
{
    /// <summary>
    /// Edit user ok.
    /// </summary>    
    OK,

    /// <summary>
    /// asp net core IdentityError, see Errors for details.
    /// </summary>    
    IdentityError,

    /// <summary>
    /// Username cannot be changed.
    /// </summary>    
    CannotChangeUsername,

    /// <summary>
    /// User not found.
    /// </summary>    
    UserNotFound,

    PermissionsError

}

public class EditUserResponseDto
{

    /// <summary>
    /// API specific status response.
    /// </summary>    
    [Required]
    public required EditUserStatus Status { get; set; }

    /// <summary>
    /// Roles added.
    /// </summary>    
    public List<string> RolesAdded { get; set; } = new();

    /// <summary>
    /// Roles removed.
    /// </summary>    
    public List<string> RolesRemoved { get; set; } = new();

    /// <summary>
    /// List of edit user errors if any.
    /// </summary>    
    public List<string> Errors { get; set; } = new();

}
