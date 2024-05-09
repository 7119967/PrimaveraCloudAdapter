namespace PCA.Infrastructure.Repositories;

public class ProjectBudgetRepository: GenericRepository<ProjectBudget>, IProjectBudgetRepository
{
    public ProjectBudgetRepository(AppDbContext context) : base(context)
    {

    }
}