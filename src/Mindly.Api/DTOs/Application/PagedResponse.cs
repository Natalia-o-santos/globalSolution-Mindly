namespace Mindly.Api.DTOs.Application;

public sealed class PagedResponse<T>
{
    public PagedResponse(IEnumerable<T> items, int page, int pageSize, int total)
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
