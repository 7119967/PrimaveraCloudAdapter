using PCA.Core.Interfaces.Repositories;

namespace PCA.Infrastructure.Repositories;

public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(AppDbContext context) : base(context)
    {
        
    }
}