namespace PCA.Infrastructure.Services;

public class ApiEventConsumer : BackgroundService, IApiConsumer
{
    private readonly string HOST_NAME;
    private readonly WebSocketClient _webSocketClient;

    public ApiEventConsumer(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider(true);
        var scope = serviceProvider.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        HOST_NAME = configuration["PrimaveraCloudApi:hostName"]!;
        _webSocketClient = new WebSocketClient(services);
        var apiHttpClient = new ApiHttpClient(services);
        var authTokenResponse = apiHttpClient.GetAuthTokenDetails();
        _webSocketClient.SetCredentials(authTokenResponse);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var uri = $"wss://{HOST_NAME}/api/events";
            await _webSocketClient.ConnectAsync(uri);
            _webSocketClient.ReceiveMessageAsync();
            // Console.ReadLine();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in event subscription: {e}");
        }
    }
}