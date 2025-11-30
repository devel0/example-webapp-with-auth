namespace ExampleWebApp.Backend.WebApi;

public static partial class Constants
{

    public const string UNIT_TEST_DB_CONN_NAME = "UnitTest";

    public const string CONFIG_KEY_AppConfig_IsUnitTest = "AppConfig:IsUnitTest";

    public const string CONFIG_KEY_AppConfig = "AppConfig";

    /// <summary>
    /// min loop delay for worker services
    /// </summary>
    public static readonly TimeSpan WORKER_THROTTLE_INTERVAL = TimeSpan.FromSeconds(0.5);

}