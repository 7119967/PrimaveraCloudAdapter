using PCA.Core.Interfaces.Entities;

namespace PCA.Core.Utils;

public class UpdatedEvent<T> : IEvent<T>
{
    public string Message { get; } = "updated";


    public UpdatedEvent(string message)
    {
        Message = message;
    }
}
