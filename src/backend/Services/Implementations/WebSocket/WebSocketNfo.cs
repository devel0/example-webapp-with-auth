namespace ExampleWebApp.Backend.WebApi.Services;

public class WebSocketNfo
{    
    public WebSocketNfo(WebSocket webSocket)
    {
        this.webSocket = webSocket;        
    }

    public WebSocket webSocket { get; }    
    
    public SemaphoreSlim SendSem { get; } = new SemaphoreSlim(1, 1);

    public string SendTimestamp { get; set; }

}
