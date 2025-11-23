using Mindly.Api.Domain.Entities;
using Mindly.Api.DTOs.Application;
using Mindly.Api.Repositories;

namespace Mindly.Api.Services;

public interface IUserService
{
    Task<User> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken = default);
    Task<User> UpdateAsync(Guid id, UserUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<User>> SearchAsync(UserQueryParameters parameters, CancellationToken cancellationToken = default);
}

