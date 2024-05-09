namespace PCA.Core.Interfaces.Services;

public interface IApiProducer
{
    Task<string> SendAsync(string message);
}