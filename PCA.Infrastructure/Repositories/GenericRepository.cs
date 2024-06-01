namespace PCA.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected DbSet<T> Set { get; }

    protected DbContext Context { get; }

    protected GenericRepository(DbContext context)
    {
        Context = context;
        Set = context.Set<T>();
    }

    public virtual IQueryable<T> Get(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "")
    {
        IQueryable<T> queryable = Set;
        if (filter != null)
        {
            queryable = queryable.Where(filter);
        }

        string[] array = includeProperties.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string navigationPropertyPath in array)
        {
            queryable = queryable.Include(navigationPropertyPath);
        }

        IQueryable<T> result;
        if (orderBy == null)
        {
            result = queryable;
        }
        else
        {
            IQueryable<T> queryable2 = orderBy(queryable);
            result = queryable2;
        }

        return result;
    }

    public virtual IQueryable<T> GetNoTracking(
        Expression<Func<T, bool>>? filter = null, 
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, 
        string includeProperties = "")
    {
        IQueryable<T> source = Set;
        if (filter != null)
        {
            source = source.Where(filter);
        }

        string[] array = includeProperties.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string navigationPropertyPath in array)
        {
            source = source.Include(navigationPropertyPath);
        }

        IQueryable<T> result;
        if (orderBy == null)
        {
            result = source.AsNoTracking();
        }
        else
        {
            IQueryable<T> queryable = orderBy(source.AsNoTracking());
            result = queryable;
        }

        return result;
    }

    public virtual IQueryable<T> GetTracking(
        Expression<Func<T, bool>>? filter = null, 
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, 
        string includeProperties = "")
    {
        IQueryable<T> source = Set;
        if (filter != null)
        {
            source = source.Where(filter);
        }

        string[] array = includeProperties.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string navigationPropertyPath in array)
        {
            source = source.Include(navigationPropertyPath);
        }

        IQueryable<T> result;
        if (orderBy == null)
        {
            result = source;  // No AsNoTracking call here
        }
        else
        {
            IQueryable<T> queryable = orderBy(source);
            result = queryable;
        }

        return result;
    }
    
    public virtual PagedResult<T> Paging(int page, int pageSize, Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string includeProperties = "")
    {
        IQueryable<T> queryable = Set;
        if (filter != null)
        {
            queryable = queryable.Where(filter);
        }

        string[] array = includeProperties.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string navigationPropertyPath in array)
        {
            queryable = queryable.Include(navigationPropertyPath);
        }

        return (orderBy != null) ? orderBy(queryable).GetPaged(page, pageSize) : queryable.GetPaged(page, pageSize);
    }

    public virtual async Task<T> GetById(object id, CancellationToken cancellationToken)
    {
        return (await Set.FindAsync(new object[1] { id }!, cancellationToken))!;
    }

    public Task Drop()
    {
        throw new NotImplementedException("Delete");
    }

    public virtual async Task<IEvent<T>> Delete(object id, CancellationToken cancellationToken)
    {
        return await Delete((await Set.FindAsync(new object[1] { id }!, cancellationToken))!, cancellationToken);
    }

    public virtual async Task<IEvent<T>> Delete(T entityToDelete, CancellationToken cancellationToken)
    {
        try
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                Set.Attach(entityToDelete);
            }

            Set.Remove(entityToDelete);
            await Context.SaveChangesAsync(cancellationToken);
            return (IEvent<T>)(object)new DeletedEvent<T>("saved");
        }
        catch (Exception ex)
        {
            Exception exception = ex;
            return (IEvent<T>)(object)new ExceptionEvent<T>(exception.Trace());
        }
    }

    public virtual async Task<IEvent<T>> Update(T entityToUpdate, CancellationToken cancellationToken)
    {
        try
        {
            //Context.Entry(entityToUpdate).State = EntityState.Modified;
            if (Context.Entry(entityToUpdate).State == EntityState.Detached)
            {
                Context!.ChangeTracker.Clear();
                await UpdateAsync(entityToUpdate);
            }
            else
            {
                Set.Attach(entityToUpdate);
                Context.Entry(entityToUpdate).State = EntityState.Modified;
            }

            await Context.SaveChangesAsync(cancellationToken);
            return (IEvent<T>)(object)new UpdatedEvent<T>("saved");
        }
        catch (Exception ex)
        {
            Exception exception = ex;
            return (IEvent<T>)(object)new ExceptionEvent<T>(exception.Trace());
        }
    }
    
    private async Task UpdateAsync(T item)
    {
        await Task.Run(() => Set.Update(item));
    }

    public virtual async Task<IEvent<T>> Insert(T entity, CancellationToken cancellationToken)
    {
        try
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                Context!.ChangeTracker.Clear();
                await Set.AddAsync(entity, cancellationToken);
            }
            else
            {
                await Set.AddAsync(entity, cancellationToken);
            }
           
            await Context.SaveChangesAsync(cancellationToken);
            return (IEvent<T>)(object)new InsertedEvent<T>(entity);
        }
        catch (Exception ex)
        {
            Exception exception = ex;
            return (IEvent<T>)(object)new ExceptionEvent<T>(exception.Trace());
        }
    }

    public Task InsertMany(T[] entities)
    {
        throw new NotImplementedException("InsertMany");
    }
    
    public async Task<long> Total(CancellationToken cancellationToken)
    {
        return await Set.CountAsync(cancellationToken);
    }   
    
    public async Task<IList<T>> GetAll(CancellationToken cancellationToken)
    {
        return await Set.ToListAsync(cancellationToken);
    }
}
