using Mindly.Api.DTOs.Application;
using Mindly.Api.Domain.Entities;

namespace Mindly.Api.Repositories;

public interface IFocusSessionRepository
{
    Task<PagedResult<FocusSession>> SearchAsync(FocusSessionQueryParameters parameters, CancellationToken cancellationToken = default);
    Task<FocusSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(FocusSession session, CancellationToken cancellationToken = default);
    void Update(FocusSession session);
    void Remove(FocusSession session);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
