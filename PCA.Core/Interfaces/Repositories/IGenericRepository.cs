using PCA.Core.Interfaces.Entities;

namespace PCA.Core.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> Get(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "");

    IQueryable<T> GetNoTracking(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "");

    IQueryable<T> GetTracking(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "");
    
    PagedResult<T> Paging(int page, int pageSize, Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "");

    Task<T> GetById(object id, CancellationToken cancellationToken);
    
    Task<IEvent<T>> Delete(object id, CancellationToken cancellationToken);

    Task<IEvent<T>> Delete(T entityToDelete, CancellationToken cancellationToken);

    Task<IEvent<T>> Update(T entityToUpdate, CancellationToken cancellationToken);
    
    Task<IEvent<T>> Insert(T entity, CancellationToken cancellationToken);

    Task InsertMany(T[] entities);
    
    Task<long> Total(CancellationToken cancellationToken);
    
    Task<IList<T>> GetAll(CancellationToken cancellationToken);
}