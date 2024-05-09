using PCA.Core.Interfaces.Entities;

namespace PCA.Core.Utils;

public class InsertedEvent<T> : IEvent<T>
{
    public string Message { get; } = "inserted";


    public T Object { get; }

    public InsertedEvent(T @object)
    {
        Object = @object;
    }
}
