var cts = new CancellationTokenSource();

var builder = WebApplication.CreateBuilder(args);

// merge user secrets to app configuration
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());

// configure database connection string and provider
builder.ConfigureDatabase();

// compress response
builder.SetupCompression();

// customize cookie name and Secure, HttpOnly, SameSite strict options
builder.SetupApplicationCookie();

// add Identity provider with custom ApplicationUser and system IdentityRole role management
builder.Services.SetupIdentityProvider();

string? corsAllowedOrigins = null;

// add cors, in development environment, for given corsAllowedOrigins
builder.SetupSecurityPolicies(corsAllowedOrigins);

// add JWT bearer authentication with handling of failed authentication to resume within refresh token
builder.SetupJWTAuthentication();

// Add services to the container.
builder.Services.AddScoped(typeof(CancellationToken), sp => cts.Token);
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUtilService, UtilService>();

// setup default roles admin, user, advanced
builder.Services.SetupRoles();

// add controllers
builder.Services.SetupControllers();

// setup json serialization behaviors
builder.Services.SetupJson();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenCustom();

// setup client static files serving on production
if (builder.Environment.IsProduction())
{
    builder.Services.AddSpaStaticFiles(configuration =>
    {
        configuration.RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "clientapp");
    });
}

//----------------------------------------------------------------------------------
// APP BUILD
//----------------------------------------------------------------------------------

var app = builder.Build();

// use response compression
app.SetupCompression();

// connects host application lifetime started, stopping, stopped magaging cancellation of given token source
app.SetupLifetime(cts);

// custommize exception handling
app.SetupException();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (corsAllowedOrigins is not null)
{
    app.UseCors(APP_CORS_POLICY_NAME);
}

app.UseHttpsRedirection();

// adds authentication middleware
app.UseAuthentication();

// adds routing middleware
app.UseRouting();

// adds authorization middleware
app.UseAuthorization();

// asp net core minimal apis configuration
app.ConfigApis();

// add endpoints controller related
app.MapControllers();

// auto apply database pending migrations
await app.ApplyDatabaseMigrations(cts.Token);

// seed database admin user and roles
await app.InitializeDatabaseAsync(cts.Token);

// adds missing roles to database from ROLES_ALL array source
await app.UpgradeRolesAsync();

// builting serve client static files from /app
if (app.Environment.IsProduction())
{
    var spaPath = "/app";

    app.Map(new PathString(spaPath), client =>
    {
        client.UseSpaStaticFiles();
        client.UseSpa(spa =>
        {
            spa.Options.SourcePath = "clientapp";
            spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    var headers = ctx.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        NoCache = true,
                        NoStore = true,
                        MustRevalidate = true
                    };
                }
            };
        });
    });
}

//----------------------------------------------------------------------------------
// APP START
//----------------------------------------------------------------------------------

// start app
await app.StartAsync();

app.Logger.LogInformation($"Listening on {string.Join(" ", app.Urls.Select(w => w.ToString()))}");

// wait for app graceful shutdown
await app.WaitForShutdownAsync();

// app run
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
