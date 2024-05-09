namespace PCA.Core.Interfaces.Services;

public interface IMessageProcessor
{
    Task ProcessMessageAsync(string json);
}