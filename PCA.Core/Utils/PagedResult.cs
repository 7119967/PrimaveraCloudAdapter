namespace PCA.Core.Utils;

public class PagedResult<T> : PagedResultBase where T : class
{
    public IList<T> Results { get; set; }

    public PagedResult()
    {
        Results = new List<T>();
    }

    public PagedResult(Exception exception)
    {
        Results = new List<T>();
        base.Message = exception.Trace();
    }
}
