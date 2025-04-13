namespace ExampleWebApp.Backend.Test;

public static partial class Constants
{

    public static readonly string AuthApiPrefix = $"{API_PREFIX}/{nameof(AuthController).StripEnd("Controller")}";

}