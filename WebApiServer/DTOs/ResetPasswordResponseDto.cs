namespace ExampleWebApp.Backend.WebApi;

public enum ResetLostPasswordStatus
{
    /// <summary>
    /// Reset lost password email sent.
    /// </summary>    
    OK,

    /// <summary>
    /// email not found.
    /// </summary>    
    NotFound,

    EmailServerError,

    InvalidToken

}

public class ResetLostPasswordResponseDto
{

    /// <summary>
    /// API specific status response.
    /// </summary>    
    [Required]
    public required ResetLostPasswordStatus Status { get; set; }     

    /// <summary>
    /// List of errors if any.
    /// </summary>    
    public List<string> Errors { get; set; } = new();

}
