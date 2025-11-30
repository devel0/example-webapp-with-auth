namespace ExampleWebApp.Backend.WebApi.Services;

public enum ExampleWSProtocolType
{
    MyProto1,
    MyProto2,
    SrvMem
}

public class ExampleWSProtocol : BaseWSProtocol
{
    /// <summary>
    /// already set if use some <see cref="ExampleWSProtocol"/>  derived types
    /// </summary>    
    public ExampleWSProtocolType ProtocolType { get; set; }
}

public class ExampleWSProto1 : ExampleWSProtocol
{

    public string? SomeMsg { get; set; }

    public ExampleWSProto1()
    {
        this.ProtocolType = ExampleWSProtocolType.MyProto1;
    }

}

public class ExampleWSProto2 : ExampleWSProtocol
{

    public long? SomeLongValue { get; set; }

    public ExampleWSProto2()
    {
        this.ProtocolType = ExampleWSProtocolType.MyProto2;
    }

}

public class ExampleWSProtoServerMem : ExampleWSProtocol
{

    /// <summary>
    /// srv memory used in bytes
    /// </summary>
    public long? MemoryUsed { get; set; }

    public ExampleWSProtoServerMem()
    {
        this.ProtocolType = ExampleWSProtocolType.SrvMem;
    }

}