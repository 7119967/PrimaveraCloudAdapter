using PCA.Core.Interfaces.Repositories;

namespace PCA.Infrastructure.Repositories;

public class EventNotificationRepository: GenericRepository<EventNotification>, IEventNotificationRepository
{
    public EventNotificationRepository(AppDbContext context) : base(context)
    {

    }
}