namespace ExampleWebApp.Backend.WebApi.Services.Auth.DTOs;

public class DeleteUserRequestDto
{
    
    [Required]
    public required string UsernameToDelete { get; set; }

}
