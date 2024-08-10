namespace ExampleWebApp.Backend.WebApi;  

public static partial class Extensions
{

    public static void SetupLogger(this WebApplicationBuilder builder)
    {

        builder.Host.UseSerilog((context, sp, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
            configuration.Enrich.FromLogContext();            

            // configuration.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {ThreadNfo2} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            configuration.WriteTo.Async(wt =>
            {
                // Source context useful to know which namespace for logger filtering
                
                wt.Console(
                    new ExpressionTemplate(
                        "[{@t:HH:mm:ss} {@l:u3}]" +                        
                        // "{#if SourceContext is not null} ({SourceContext}){#end}" +                        
                        " {@m}\n{@x}",
                        theme: TemplateTheme.Code));
            });

        });
    }

}