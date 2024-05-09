namespace PCA.Infrastructure.Services;

public class WebSocketClient : IWebSocketClient
{
    private readonly ClientWebSocket _webSocket;
    private readonly MessageProcessor _messageProcessor;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public WebSocketClient(IServiceCollection services)
    {
        _webSocket = new ClientWebSocket();
        _messageProcessor = new MessageProcessor(services);
    }

    public void SetCredentials(Dictionary<string, object> authTokenResponse)
    {
        var tokenAuth = $"Bearer {authTokenResponse["accessToken"]}";

        _webSocket.Options.SetRequestHeader("Authorization", tokenAuth);
        //_webSocket.Options.SetRequestHeader("x-prime-tenant", authTokenResponse["requestHeaders:x-prime-tenant"].ToString());
        //_webSocket.Options.SetRequestHeader("x-prime-region", authTokenResponse["requestHeaders:x-prime-region"].ToString());
        //_webSocket.Options.SetRequestHeader("x-prime-identity-app", authTokenResponse["requestHeaders:x-prime-identity-app"].ToString());

        var headers = authTokenResponse["requestHeaders"] as JObject;
        foreach (var header in headers!)
        {
            _webSocket.Options.SetRequestHeader(header.Key, header.Value!.ToString());
        }
    }

    public async Task ConnectAsync(string uri)
    {
        try
        {
            await _webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection error: {ex.Message}");
        }
    }

    public async Task SendMessageAsync(string message)
    {
        var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        if (_webSocket.State == WebSocketState.Open)
        {
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        else
        {
            Console.WriteLine($"Connection error: ");
        }
    }

    public async void ReceiveMessageAsync()
    {
        while (_webSocket.State == WebSocketState.Open)
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            var result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer.Array!, 0, result.Count);
                await ProcessMessage(message);
            }
        }
    }

    private async Task ProcessMessage(string message)
    {
        try
        {
            await _messageProcessor.ProcessMessageAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while processing the message: {message}");
        }
    }

    public async Task CloseAsync()
    {
        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    }
}