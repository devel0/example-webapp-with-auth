namespace ExampleWebApp.Backend.WebApi.Services;

public enum BaseWSProtocolType
{
    Custom,

    Ping,

    Pong,

}

[GenerateSwaggerSchema]
public class BaseWSProtocol(BaseWSProtocolType baseMessageType)
{
    public BaseWSProtocolType BaseProtocolType { get; } = baseMessageType;
}

public class WSPing : BaseWSProtocol
{

    public WSPing(string msg) : base(BaseWSProtocolType.Ping)
    {
        Msg = msg;
    }

    public string Msg { get; set; }

}

public class WSPong : BaseWSProtocol
{

    public WSPong(string msg) : base(BaseWSProtocolType.Pong)
    {
        Msg = msg;
    }

    public string Msg { get; set; }

}
