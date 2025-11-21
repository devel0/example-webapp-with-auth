namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// Send given string to the websocket.
    /// </summary>    
    public static async Task<bool> SendStringAsync(this WebSocket ws, string str, CancellationToken cancellationToken)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
        if (ws.State == WebSocketState.Open)
            await ws.SendAsync(arraySegment, WebSocketMessageType.Text, true, cancellationToken);

        else if (ws.State == WebSocketState.Closed || ws.State == WebSocketState.Aborted)
            return false;

        return true;
    }

    /// <summary>
    /// Receive from websocket returning string ( max 2gb due to use of backing memorystream )
    /// </summary>    
    public static async Task<string?> ReceiveStringAsync(this WebSocket ws, CancellationToken cancellationToken)
    {
        string? res = null;

        try
        {
            var ms = new MemoryStream();

            WebSocketReceiveResult? wsr = null;
            var buffer = new ArraySegment<byte>(new byte[1024 * 4]);
            if (buffer.Array is null) return null;

            do
            {
                wsr = await ws.ReceiveAsync(buffer, cancellationToken);
                ms.Write(buffer.Array, buffer.Offset, wsr.Count);
            } while (!wsr.EndOfMessage);

            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            res = Encoding.UTF8.GetString(ms.ToArray());
        }

        catch (WebSocketException wex) when (wex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
        {            
        }

        return res;
    }

}