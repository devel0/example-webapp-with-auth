namespace ExampleWebApp.Backend.WebApi.Types;

public class CustomModelDocumentFilter<T> : IDocumentFilter //where T : class, struct
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var type = typeof(T);
        context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
    }
}
