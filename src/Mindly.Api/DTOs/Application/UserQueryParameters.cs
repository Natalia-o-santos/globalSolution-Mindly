namespace Mindly.Api.DTOs.Application;

public sealed class UserQueryParameters
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "createdAt";
    public bool Descending { get; set; } = true;
}

