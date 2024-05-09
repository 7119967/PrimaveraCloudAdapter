namespace PCA.Core.Interfaces;

public interface IEntityFactory
{
    EventNotification CreateEntity(Dictionary<string, object> data);
}