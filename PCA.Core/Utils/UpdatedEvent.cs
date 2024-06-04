namespace PCA.Core.Utils;

public class UpdatedEvent<T> : IEvent<T>
{
    public string Message { get; } = "updated";

    public T Object { get; }
    public UpdatedEvent(T @object, string message = "")
    {
        Message = message;
        Object = @object;
    }
}
