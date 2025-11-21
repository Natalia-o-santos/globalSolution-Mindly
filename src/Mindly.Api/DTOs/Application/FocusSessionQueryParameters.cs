using Mindly.Api.Domain.Enums;

namespace Mindly.Api.DTOs.Application;

public sealed class FocusSessionQueryParameters
{
    public string? Title { get; set; }
    public FocusSessionStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "createdAt";
    public bool Descending { get; set; } = true;
}
