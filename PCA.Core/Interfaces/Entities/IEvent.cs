namespace PCA.Core.Interfaces.Entities;

public interface IEvent<T>
{
    string Message { get; }
}
