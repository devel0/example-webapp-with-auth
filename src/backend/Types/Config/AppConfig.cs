namespace ExampleWebApp.Backend.WebApi.Types;

public enum DbProviderConfig
{
    Postgres,
    Mysql
};

/// <summary>
/// appsettings.json "AppConfig" object
/// </summary>
public class AppConfig
{

    /// <summary>
    /// change a value in <see cref="AppConfig"/> reflecting to the configuration section
    /// </summary>
    public static void SetValue<P, T>(IConfiguration configuration, Expression<Func<AppConfig, P>> path, T value)
    {
        var Q = ConfigurationPropertyPathHelper<AppConfig>.PropertyPath(path);

        configuration.GetSection(Q).Value = value?.ToString() ?? "";
    }

    public ServerConfig Server { get; set; } = new();

    public AuthConfig Auth { get; set; } = new();

    public DatabaseConfig Database { get; set; } = new();

    public DbProviderConfig? DatabaseProvider
    {
        get
        {
            var connName = Database.ConnectionName;
            var q = Database.Connections.FirstOrDefault(w => w.Name == connName);
            return q?.Provider;
        }
    }

    public EmailServerConfig EmailServer { get; set; } = new();

    public FakeDataSetConfig FakeDataSet { get; set; } = new();

    public bool IsUnitTest { get; set; }

    //-------------------------------

    /// <summary>
    /// appsettings.json ( AppConfig:Server )
    /// </summary>
    public class ServerConfig
    {

        public string HostName { get; set; } = "";

    }

    /// <summary>
    /// appsettings.json ( AppConfig:Auth )
    /// </summary>
    public class AuthConfig
    {

        // public void SetValue<T,V>(Func<V> 

        public JwtConfig Jwt { get; set; } = new();

        /// <summary>
        /// appsettings.json ( AppConfig:Auth:Jwt )
        /// </summary>
        public class JwtConfig
        {

            public string Issuer { get; set; } = "";
            public string Audience { get; set; } = "";
            public TimeSpan AccessTokenDuration { get; set; }
            public TimeSpan RefreshTokenDuration { get; set; }
            public TimeSpan ClockSkew { get; set; }
            public string Key { get; set; } = "";

        }

    }

    public class DatabaseConfig
    {

        /// <summary>
        /// if true produce snakecase mode tables ( useful for postgres db provider )
        /// </summary>
        public bool SchemaSnakeCase { get; set; }

        /// <summary>
        /// connection to use between those available in <see cref="Connections"/> 
        /// </summary>
        public string ConnectionName { get; set; } = "";

        public List<ConnectionItemConfig> Connections { get; set; } = new();

        public SeedConfig Seed { get; set; } = new();

        public class ConnectionItemConfig
        {

            public string Name { get; set; } = "";
            public DbProviderConfig Provider { get; set; }
            public string ConnectionString { get; set; } = "";            

        }

        public class SeedConfig
        {

            public List<UserConfig> Users { get; set; } = new();

            public class UserConfig
            {
                public string Username { get; set; } = "";
                public string Password { get; set; } = "";
                public string Email { get; set; } = "";
                public List<string> Roles { get; set; } = new();
            }

        }

    }

    /// <summary>
    /// email server settings to manage password reset link
    /// </summary>
    public class EmailServerConfig
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string SmtpServer { get; set; } = "";
        public int SmtpServerPort { get; set; }
        public SecurityEnum Security { get; set; }
        public string FromDisplayName { get; set; } = "";

        public enum SecurityEnum
        {
            None,
            Auto,
            Ssl,
            Tls
        }
    }

    public class FakeDataSetConfig
    {
        /// <summary>
        /// if not null the seeding of fakedata will saved to given csv pathfilename
        /// </summary>        
        public string? SaveToCsvPathfilename { get; set; }

        /// <summary>
        /// if not null the seeding of fakedata will be loaded from given csv pathfilename
        /// </summary>
        public string? LoadFromCsvPathfilename { get; set; }
    }

}