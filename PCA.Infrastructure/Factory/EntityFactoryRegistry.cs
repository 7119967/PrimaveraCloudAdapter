namespace PCA.Infrastructure.Factory;

public class EntityFactoryRegistry
{
    private readonly Dictionary<string, IEntityFactory> factories = new Dictionary<string, IEntityFactory>();

    public void RegisterFactory(string entityType, IEntityFactory factory)
    {
        factories.Add(entityType, factory);
    }

    public EventNotification CreateEntity(string entityType, Dictionary<string, object> data)
    {
        if (factories.TryGetValue(entityType, out var factory))
        {
            return factory.CreateEntity(data);
        }
        // Обработка случая, когда фабрика для указанного типа не найдена
        return null;
    }
}