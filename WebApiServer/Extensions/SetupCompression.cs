namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    public static IServiceCollection SetupCompression(this WebApplicationBuilder builder)
    {
        var res = builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        builder.Services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.SmallestSize;
        });

        return res;
    }

    public static IApplicationBuilder SetupCompression(this WebApplication app)
    {
        app.UseResponseCompression();

        return app;

    }
    
}