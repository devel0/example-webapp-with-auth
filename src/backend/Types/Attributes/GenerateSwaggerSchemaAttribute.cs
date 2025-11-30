namespace ExampleWebApp.Backend.WebApi.Types;

/// <summary>
/// decorate types to export with swagger.
/// Useful when type isn't directly referenced by some controller api
/// </summary>
public class GenerateSwaggerSchemaAttribute : Attribute
{
}