namespace ExampleWebApp.Backend.WebApi.Services;

public enum BaseWSProtocolType
{
    Custom,

    Ping,

    Pong,

}

[GenerateSwaggerSchema]
public class BaseWSProtocol
{

    /// <summary>
    /// (don't care) : for internal protocol purpose
    /// </summary>    
    public BaseWSProtocolType BaseProtocolType { get; set; } = BaseWSProtocolType.Custom;

    /// <summary>
    /// (don't care) : for internal protocol purpose
    /// </summary>
    public string? BaseProtocolMsg { get; set; }

}
