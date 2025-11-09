namespace ExampleWebApp.Backend.WebApi.Types;

public class CustomModelDocumentFilter<T> : IDocumentFilter //where T : class, struct
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var type = typeof(T);
        context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
    }
}

public class GenerateSwaggerSchemaDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var attributeType = typeof(GenerateSwaggerSchemaAttribute);

        var assembly = Assembly.GetAssembly(typeof(Program));

        if (assembly is not null)
        {

            var types = assembly
                .GetTypes()
                .Where(t => t.GetCustomAttribute<GenerateSwaggerSchemaAttribute>(inherit: true) != null)
                .ToList();

            foreach (var type in types)
            {
                context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
            }

        }
    }
}