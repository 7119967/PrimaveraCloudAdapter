namespace PCA.Infrastructure.Services.WebSocket;

public class WebSocketClient : IWebSocketClient
{
    private readonly ClientWebSocket _webSocket;
    private readonly MessageProcessor _messageProcessor;
    private readonly ILogger<WebSocketClient> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public WebSocketClient(IServiceCollection services)
    {
        _webSocket = new ClientWebSocket();
        _messageProcessor = new MessageProcessor(services);
         var sp = services.BuildServiceProvider();
         _logger = sp.GetRequiredService<ILogger<WebSocketClient>>();
    }

    public void SetCredentials(Dictionary<string, object> authTokenResponse)
    {
        var tokenAuth = $"Bearer {authTokenResponse["accessToken"]}";
        _webSocket.Options.SetRequestHeader("Authorization", tokenAuth);
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
            _logger.LogInformation($"Connection error: {ex.Message}");
            throw new BaseException($"Connection error: {ex.Message}");
        }
    }

    public async Task SendMessageAsync(string message)
    {
        try
        {
            var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"Connection error: {ex.Message}");
            throw new BaseException($"Connection error: {ex.Message}");
        }
    }

    public async void ReceiveMessageAsync()
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogInformation($"Connection error: {ex.Message}");
            throw new BaseException($"Connection error: {ex.Message}");
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
            _logger.LogInformation($"An error occurred while processing the message: {ex.Message}");
            throw new BaseException($"An error occurred while processing the message: {ex.Message}");
        }
    }

    public async Task CloseAsync()
    {
        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    }
}