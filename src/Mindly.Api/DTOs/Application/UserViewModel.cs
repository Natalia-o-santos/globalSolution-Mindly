using Mindly.Api.Domain.Entities;

namespace Mindly.Api.DTOs.Application;

public class UserViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<LinkDto> Links { get; set; } = new();
}

