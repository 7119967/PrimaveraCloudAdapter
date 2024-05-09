using PCA.Core.Interfaces.Entities;

namespace PCA.Core.Utils;

public class ExceptionEvent<T> : IEvent<T>
{
    public string Message { get; } = "failed";


    public ExceptionEvent(string message)
    {
        Message = message;
    }
}
