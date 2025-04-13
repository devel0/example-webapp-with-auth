namespace ExampleWebApp.Backend.Test;

public record UserCredentialNfo(string username, string email, string password, string testPrefix);

public record ExpectedPermissionsNfo(string username, string password, UserPermission[] permissions);