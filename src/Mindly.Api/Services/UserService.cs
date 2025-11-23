using Mindly.Api.Domain.Entities;
using Mindly.Api.Domain.Exceptions;
using Mindly.Api.DTOs.Application;
using Mindly.Api.Repositories;

namespace Mindly.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<User> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken = default)
    {
        var existingUser = await _repository.GetByEmailAsync(dto.Email, cancellationToken);
        if (existingUser is not null)
        {
            throw new DomainValidationException("Já existe um usuário com este email.");
        }

        var user = new User(dto.Name, dto.Email);
        await _repository.AddAsync(user, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User> UpdateAsync(Guid id, UserUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            throw new DomainValidationException("Usuário não encontrado.");
        }

        var existingUser = await _repository.GetByEmailAsync(dto.Email, cancellationToken);
        if (existingUser is not null && existingUser.Id != id)
        {
            throw new DomainValidationException("Já existe outro usuário com este email.");
        }

        user.Update(dto.Name, dto.Email);
        _repository.Update(user);
        await _repository.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            throw new DomainValidationException("Usuário não encontrado.");
        }

        _repository.Remove(user);
        await _repository.SaveChangesAsync(cancellationToken);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdAsync(id, cancellationToken);
    }

    public Task<PagedResult<User>> SearchAsync(UserQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        return _repository.SearchAsync(parameters, cancellationToken);
    }
}

