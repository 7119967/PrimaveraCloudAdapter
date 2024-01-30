namespace PCA.Core.Services;

public class WebSocketClient
{
   private static readonly string USER_NAME = "<userName>";
    private static readonly string PASSWORD = "<password>";
    private static readonly string HOST_NAME = "<hostName>";
    private static readonly string OBJECT = "ApiEntityProject";
    private static readonly string[] EVENTS = { "CREATE", "UPDATE", "DELETE" };
    private static readonly string FILTERS = "projectName=*Construction";

    private ClientWebSocket _webSocket;

    public WebSocketClient(JsonNode authTokenResponse)
    {
        _webSocket = new ClientWebSocket();
        SetCredentials(authTokenResponse);
    }

    public async Task ConnectAsync()
    {
        Uri uri = new Uri($"wss://{HOST_NAME}/api/events");
        await _webSocket.ConnectAsync(uri, CancellationToken.None);


        ListenForMessages();
    }

    private void SetCredentials(JsonNode authTokenResponse)
    {
        var tokenAuth = $"Bearer {authTokenResponse["accessToken"]}";

        _webSocket.Options.SetRequestHeader("Authorization", tokenAuth);
        _webSocket.Options.SetRequestHeader("x-prime-tenant-code", authTokenResponse["primeTenantCode"].ToString());

        var headers = authTokenResponse["requestHeaders"] as Dictionary<string, string>;
        foreach (var header in headers!)
        {
            _webSocket.Options.SetRequestHeader(header.Key, header.Value);
        }
    }

    private async void ListenForMessages()
    {
        try
        {
            while (_webSocket.State == WebSocketState.Open)
            {
                var buffer = new ArraySegment<byte>(new byte[8192]);
                var result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                    ProcessMessage(message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in listening for messages: {ex.Message}");
        }
    }

    private void ProcessMessage(string message)
    {
        try
        {
            dynamic json = JsonConvert.DeserializeObject(message);
            Console.WriteLine("<-Message from Primavera Cloud is:");
            Console.WriteLine(JsonConvert.SerializeObject(json, Formatting.Indented));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"<-Message from Primavera Cloud is: {message}");
        }
    }
}
