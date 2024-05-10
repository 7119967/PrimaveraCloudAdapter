namespace PCA.Infrastructure.Repositories;

public class ObjectTypeRepository : GenericRepository<ObjectType>, IObjectTypeRepository
{
    public ObjectTypeRepository(AppDbContext context) : base(context)
    {

    }
}
