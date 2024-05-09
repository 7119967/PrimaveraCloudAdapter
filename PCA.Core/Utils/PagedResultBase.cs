namespace PCA.Core.Utils;

public abstract class PagedResultBase
{
    public int CurrentPage { get; set; }

    public int PageCount { get; set; }

    public int PageSize { get; set; }

    public int RowCount { get; set; }

    public int FirstRowOnPage => (CurrentPage - 1) * PageSize + 1;

    public string Message { get; protected set; } = "Ok";

    public int LastRowOnPage => Math.Min(CurrentPage * PageSize, RowCount);
}
