namespace PCA.Core.Services;

public class WebSocketClient
{
    private string HOST_NAME;
    private int HOST_PORT;
    private string USER_NAME;
    private string PASSWORD;
    private string[] OBJECT;
    private string[] EVENTS;
    private string FILTERS;
    private bool subscription;

    private ClientWebSocket _webSocket;

    public WebSocketClient(JsonNode authTokenResponse, IServiceScopeFactory scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        if (env.IsProduction())
        {
            HOST_NAME = configuration["PrimaveraCloudApi:hostName"]!;
            HOST_PORT = Convert.ToInt32(configuration["PrimaveraCloudApi:port"])!;
            USER_NAME = configuration["PrimaveraCloudApi:userName"]!;
            PASSWORD = configuration["PrimaveraCloudApi:password"]!;
            OBJECT = (string[]?)(object)configuration["PrimaveraCloudApi:entityObjectType"]!;
            EVENTS = (string[]?)(object)configuration["PrimaveraCloudApi:eventTypes"]!;
            FILTERS = configuration["PrimaveraCloudApi:password"]!;
            subscription = Convert.ToBoolean(configuration["PrimaveraCloudApi:subscription"])!;
        }
        else 
        {
            HOST_NAME = configuration["PrimaveraCloudApi:hostName"]!;
            HOST_PORT = Convert.ToInt32(configuration["PrimaveraCloudApi:port"])!;
            USER_NAME = configuration["PrimaveraCloudApi:userName"]!;
            PASSWORD = configuration["PrimaveraCloudApi:password"]!;
            OBJECT = (string[]?)(object)configuration["PrimaveraCloudApi:entityObjectType"]!;
            EVENTS = (string[]?)(object)configuration["PrimaveraCloudApi:eventTypes"]!;
            FILTERS = configuration["PrimaveraCloudApi:password"]!;
            subscription = Convert.ToBoolean(configuration["PrimaveraCloudApi:subscription"])!;
        }



        _webSocket = new ClientWebSocket();
        SetCredentials(authTokenResponse);
    }

    public async Task ConnectAsync()
    {
        Uri uri = new Uri($"wss://{HOST_NAME}:{HOST_PORT}/api/events");
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
