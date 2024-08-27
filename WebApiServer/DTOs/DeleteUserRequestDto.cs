namespace ExampleWebApp.Backend.WebApi; 

public class DeleteUserRequestDto
{
    
    [Required]
    public required string UsernameToDelete { get; set; }

}
