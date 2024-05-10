namespace PCA.Infrastructure.Repositories;

public class ActivityRelationshipRepository: GenericRepository<ActivityRelationship>, IActivityRelationshipRepository
{
    public ActivityRelationshipRepository(AppDbContext context) : base(context)
    {

    }
}