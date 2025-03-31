namespace ExampleWebApp.Backend.WebApi.Services.Abstractions.Auth.DTOs;

public class DeleteUserRequestDto
{
    
    [Required]
    public required string UsernameToDelete { get; set; }

}
