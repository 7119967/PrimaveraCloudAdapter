namespace PCA.Infrastructure.Repositories;

public class WbsRepository: GenericRepository<Wbs>, IWbsRepository
{
    public WbsRepository(AppDbContext context) : base(context)
    {

    }
}