namespace PCA.Infrastructure.Repositories;

public class ActivityRepository: GenericRepository<Activity>, IActivityRepository
{
    public ActivityRepository(AppDbContext context) : base(context)
    {

    }
}