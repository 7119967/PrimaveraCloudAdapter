namespace PCA.Infrastructure.Factory;

public class ApiEntityProjectFactory: IEntityFactory
{
    public EventNotification CreateEntity(Dictionary<string, object> data)
    {
        var entity = new EventNotification
        {
            // Заполните свойства объекта на основе данных
        };
        return entity;
    }
}