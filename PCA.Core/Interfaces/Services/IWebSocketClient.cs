namespace PCA.Core.Interfaces.Services;

public interface IWebSocketClient
{
    Task ConnectAsync(string uri);
    Task SendMessageAsync(string message);
    void ReceiveMessageAsync();
}