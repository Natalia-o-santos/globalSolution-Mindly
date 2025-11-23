using Mindly.Api.DTOs.Application;
using Mindly.Api.Domain.Entities;

namespace Mindly.Api.Repositories;

public interface IUserRepository
{
    Task<PagedResult<User>> SearchAsync(UserQueryParameters parameters, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    void Update(User user);
    void Remove(User user);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

