namespace Mindly.Api.Repositories;

public sealed class PagedResult<T>
{
    public PagedResult(IEnumerable<T> items, int page, int pageSize, int total)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        Total = total;
        TotalPages = (int)Math.Ceiling(total / (double)pageSize);
    }

    public int Page { get; }
    public int PageSize { get; }
    public int Total { get; }
    public int TotalPages { get; }
    public IEnumerable<T> Items { get; }
}
