using PCA.Core.Interfaces.Repositories;

namespace PCA.Infrastructure.Repositories;

public class ProjectRepository: GenericRepository<Project>, IProjectRepository
{
    public ProjectRepository(AppDbContext context) : base(context)
    {

    }
}