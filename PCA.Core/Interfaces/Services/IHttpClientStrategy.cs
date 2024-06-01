namespace PCA.Core.Interfaces.Services;

public interface IHttpClientStrategy<T> where T : class, IDisposable
{
    Task GetDataAsync(Transaction transaction, dynamic message);
}