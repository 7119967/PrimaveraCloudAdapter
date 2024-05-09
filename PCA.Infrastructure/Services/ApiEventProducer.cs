using PCA.Infrastructure.Services.HttpClients;
using PCA.Infrastructure.Services.WebSocket;

namespace PCA.Infrastructure.Services;

public class ApiEventProducer : IApiProducer
{
    private readonly string HOST_NAME;
    private readonly WebSocketClient _webSocketClient;
    
    public ApiEventProducer(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        HOST_NAME = configuration["PrimaveraCloudApi:hostName"]!;
        _webSocketClient = new WebSocketClient(services);
        var apiHttpClient = new ApiHttpClient(services);
        var authTokenResponse = apiHttpClient.GetAuthTokenDetails();
        _webSocketClient.SetCredentials(authTokenResponse);
    }
    
    public async Task<string> SendAsync(string message)
    {
        try
        {
            var uri = $"wss://{HOST_NAME}/api/events";
            await _webSocketClient.ConnectAsync(uri);
            await _webSocketClient.SendMessageAsync(message);
            return "Message was sent successfully";
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in event subscription: {e}");
        }
        finally
        {
          _webSocketClient.ReceiveMessageAsync(); 
        }

        return null;
    }
}