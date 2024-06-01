namespace PCA.Infrastructure.Services;

public class ApiEventConsumer : BackgroundService, IApiConsumer
{
    private readonly string HOST_NAME;
    private readonly WebSocketClient _webSocketClient;
    private readonly ILogger<ApiEventConsumer> _logger;

    public ApiEventConsumer(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider(true);
        var scope = serviceProvider.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<ApiEventConsumer>>();
        HOST_NAME = configuration["PrimaveraCloudApi:HostName"]!;
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
        }
        catch (Exception e)
        {
            _logger.LogError($"Error in event subscription: {e}");
            throw new BaseException($"Error in event subscription: {e}");
        }
    }
}