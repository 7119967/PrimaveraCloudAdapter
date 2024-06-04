namespace PCA.Core.Utils;

public class DeletedEvent<T> : IEvent<T>
{
    public string Message { get; } = "deleted";


    public DeletedEvent(string message)
    {
        Message = message;
    }
}
