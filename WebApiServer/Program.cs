var cts = new CancellationTokenSource();

var builder = WebApplication.CreateBuilder(args);

// setup logger
builder.SetupLogger();

// load config from appsettings, environment and user-secrets
builder.SetupAppSettings();

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
builder.SetupSpa();

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
    app.ConfigSwagger();

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

// setup mapping for serving spa static files
app.MapSpaStaticFiles();

//----------------------------------------------------------------------------------
// APP START
//----------------------------------------------------------------------------------

// start app
await app.StartAsync();

app.Logger.LogInformation($"Environment: {app.Environment.EnvironmentName}");

app.Logger.LogInformation($"Listening on {string.Join(" ", app.Urls.Select(w => w.ToString()))}");

// wait for app graceful shutdown
await app.WaitForShutdownAsync();

// app run
app.Run();
