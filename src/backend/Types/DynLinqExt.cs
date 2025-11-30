namespace ExampleWebApp.Backend.WebApi.Types;

[DynamicLinqType]
public static class DbFun
{
    
    public static string GuidString(object guid)
        => throw new NotSupportedException();

}