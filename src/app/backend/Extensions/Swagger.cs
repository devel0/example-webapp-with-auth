namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// Enable swagger openapi with xml comments and jwt bearer auth token button.
    /// </summary>    
    public static void AddSwaggerGenCustom(this IServiceCollection serviceCollection)
    {
        serviceCollection
        .AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = SWAGGER_API_TITLE, Version = "v1" });

            // options.DocumentFilter<CustomModelDocumentFilter<SomeExportedApiAddictionalType>>();

            var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

            foreach (var xmlFile in xmlFiles)
                options.IncludeXmlComments(xmlFile);

            options.AddEnumsWithValuesFixFilters(o =>
            {
                foreach (var xmlFile in xmlFiles)
                    o.IncludedXmlCommentsPaths.Add(xmlFile);

                // add schema filter to fix enums (add 'x-enumNames' for NSwag or its alias from XEnumNamesAlias) in schema
                o.ApplySchemaFilter = true;

                // alias for replacing 'x-enumNames' in swagger document
                o.XEnumNamesAlias = "x-enum-varnames";

                // alias for replacing 'x-enumDescriptions' in swagger document
                o.XEnumDescriptionsAlias = "x-enum-descriptions";

                // add parameter filter to fix enums (add 'x-enumNames' for NSwag or its alias from XEnumNamesAlias) in schema parameters
                o.ApplyParameterFilter = true;

                // add document filter to fix enums displaying in swagger document
                o.ApplyDocumentFilter = true;

                // add descriptions from DescriptionAttribute or xml-comments to fix enums (add 'x-enumDescriptions' or its alias from XEnumDescriptionsAlias for schema extensions) for applied filters
                o.IncludeDescriptions = true;

                // add remarks for descriptions from xml-comments
                o.IncludeXEnumRemarks = true;

                // get descriptions from DescriptionAttribute then from xml-comments
                // o.DescriptionSource = DescriptionSources.DescriptionAttributesThenXmlComments;
                o.DescriptionSource = DescriptionSources.XmlComments;

                // new line for enum values descriptions
                o.NewLine = Environment.NewLine;
            });

            // enable interpret [SwaggerOperation]
            options.EnableAnnotations();
        });
    }

    /// <summary>
    /// Configure swagger theme.
    /// </summary>
    public static void ConfigSwagger(this WebApplication webApplication)
    {
        var logger = webApplication.Logger;

        webApplication.UseSwagger();
        webApplication.UseSwaggerUI(c =>
        {
            c.InjectStylesheet("/swagger/SwaggerDark.css");
        });
        webApplication.MapGet("/swagger/SwaggerDark.css", async ([FromServices] CancellationToken cancellationToken) =>
        {
            var cssPathfilename = SWAGGER_CSS_PATH();
            var css = await File.ReadAllBytesAsync(cssPathfilename, cancellationToken);
            return Results.File(css, "text/css");
        }).ExcludeFromDescription();
    }

}