using Mindly.Api.Domain.Entities;
using Mindly.Api.DTOs.Application;
using Mindly.Api.Repositories;

namespace Mindly.Api.Services;

public interface IFocusSessionService
{
    Task<FocusSession> CreateAsync(FocusSessionCreateDto dto, CancellationToken cancellationToken = default);
    Task<FocusSession> UpdateAsync(Guid id, FocusSessionUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FocusSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<FocusSession>> SearchAsync(FocusSessionQueryParameters parameters, CancellationToken cancellationToken = default);
}
