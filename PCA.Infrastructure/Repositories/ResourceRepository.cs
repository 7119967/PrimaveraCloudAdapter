namespace PCA.Infrastructure.Repositories;

public class ResourceRepository: GenericRepository<Resource>, IResourceRepository
{
    public ResourceRepository(AppDbContext context) : base(context)
    {

    }
}