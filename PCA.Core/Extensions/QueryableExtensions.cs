namespace PCA.Core.Extensions;

public static class QueryableExtensions
{
    public static PagedResult<T> GetPaged<T>(this IQueryable<T> query, int page, int pageSize) where T : class
    {
        if (page == 0)
        {
            page = 1;
        }

        if (pageSize == 0)
        {
            pageSize = 20;
        }

        PagedResult<T> pagedResult = new PagedResult<T>
        {
            CurrentPage = page,
            PageSize = pageSize,
            RowCount = query.Count()
        };
        double a = (double)pagedResult.RowCount / (double)pageSize;
        pagedResult.PageCount = (int)Math.Ceiling(a);
        int count = (page - 1) * pageSize;
        pagedResult.Results = query.Skip(count).Take(pageSize).ToList();
        return pagedResult;
    }

    public static PagedResult<T> GetPaged<T>(this IEnumerable<T> query, int page, int pageSize) where T : class
    {
        if (page == 0)
        {
            page = 1;
        }

        if (pageSize == 0)
        {
            pageSize = 20;
        }

        List<T> list = query.ToList();
        PagedResult<T> pagedResult = new PagedResult<T>
        {
            CurrentPage = page,
            PageSize = pageSize,
            RowCount = list.Count
        };
        double a = (double)pagedResult.RowCount / (double)pageSize;
        pagedResult.PageCount = (int)Math.Ceiling(a);
        int count = (page - 1) * pageSize;
        pagedResult.Results = list.Skip(count).Take(pageSize).ToList();
        return pagedResult;
    }
}
