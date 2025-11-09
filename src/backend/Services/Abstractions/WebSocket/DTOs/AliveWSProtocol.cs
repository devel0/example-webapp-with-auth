namespace ExampleWebApp.Backend.WebApi.Services.Abstractions;

public enum AliveWSMessageType
{

    Ping,

    Pong,

}

[GenerateSwaggerSchema]
public class AliveWSProtocol(AliveWSMessageType messageType)
{
    public AliveWSMessageType MessageType { get; set; } = messageType;
}

public class WSPing : AliveWSProtocol
{

    public WSPing(string msg) : base(AliveWSMessageType.Ping)
    {
        Msg = msg;
    }

    public string Msg { get; set; }

}

public class WSPong : AliveWSProtocol
{

    public WSPong(string msg) : base(AliveWSMessageType.Pong)
    {
        Msg = msg;
    }

    public string Msg { get; set; }

}
