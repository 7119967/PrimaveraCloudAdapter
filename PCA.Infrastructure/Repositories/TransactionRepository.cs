using PCA.Core.Interfaces.Repositories;

namespace PCA.Infrastructure.Repositories;

public class TransactionRepository: GenericRepository<Transaction>, ITransactionRepository
{
    public TransactionRepository(AppDbContext context) : base(context)
    {

    }
}