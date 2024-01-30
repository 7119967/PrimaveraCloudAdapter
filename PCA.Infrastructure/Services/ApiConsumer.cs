using JsonNode = PCA.Core.Domain.JsonNode;

namespace PCA.Infrastructure.Services;

public class ApiConsumer: BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var authTokenResponse = GetAuthTokenDetails();
            var websocketClient = new WebSocketClient(authTokenResponse);
            await websocketClient.ConnectAsync();
            
            Console.ReadLine();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in event subscription: {e}");
        }
    }
    
    private static JsonNode GetAuthTokenDetails()
    {
        // Implement the logic to obtain the OAuth token response
        return new JsonNode();
    }
}