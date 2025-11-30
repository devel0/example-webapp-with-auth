namespace ExampleWebApp.Backend.WebApi.Services;


public enum ExampleWSProtocolType
{    
    Mex
}

public class ExampleWSProtocol(ExampleWSProtocolType messageType) : BaseWSProtocol(BaseWSProtocolType.Custom)
{

    public ExampleWSProtocolType ProtocolType { get; set; } = messageType;
}

public class WSCustomMex : ExampleWSProtocol
{

    public string SomeMsg { get; }

    public WSCustomMex(string someMsg) : base(ExampleWSProtocolType.Mex)
    {
        this.SomeMsg = someMsg;
    }

}
