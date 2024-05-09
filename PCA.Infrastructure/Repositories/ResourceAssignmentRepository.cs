using PCA.Core.Interfaces.Repositories;

namespace PCA.Infrastructure.Repositories;

public class ResourceAssignmentRepository: GenericRepository<ResourceAssignment>, IResourceAssignmentRepository
{
    public ResourceAssignmentRepository(AppDbContext context) : base(context)
    {

    }
}