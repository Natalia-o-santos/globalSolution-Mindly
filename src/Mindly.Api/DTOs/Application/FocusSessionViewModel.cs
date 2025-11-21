using Mindly.Api.Domain.Enums;

namespace Mindly.Api.DTOs.Application;

public sealed class FocusSessionViewModel
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int FocusMinutes { get; init; }
    public int BreakMinutes { get; init; }
    public FocusSessionStatus Status { get; init; }
    public bool IoTIntegrationEnabled { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public IEnumerable<LinkDto>? Links { get; init; }
}
