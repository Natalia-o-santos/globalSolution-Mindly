using System.Linq;
using Microsoft.EntityFrameworkCore;
using Mindly.Api.Data;
using Mindly.Api.Domain.Entities;
using Mindly.Api.DTOs.Application;

namespace Mindly.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<User>> SearchAsync(UserQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var normalizedPage = Math.Max(parameters.Page, 1);
        var normalizedPageSize = Math.Clamp(parameters.PageSize, 5, 100);
        var query = _context.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Name))
        {
            var name = parameters.Name.Trim().ToLowerInvariant();
            query = query.Where(u => u.Name.ToLower().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Email))
        {
            var email = parameters.Email.Trim().ToLowerInvariant();
            query = query.Where(u => u.Email.ToLower().Contains(email));
        }

        var total = await query.CountAsync(cancellationToken);
        query = ApplySorting(query, parameters.SortBy, parameters.Descending);

        var items = await query
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<User>(items, normalizedPage, normalizedPageSize, total);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }

    public void Remove(User user)
    {
        _context.Users.Remove(user);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<User> ApplySorting(IQueryable<User> query, string? sortBy, bool descending)
    {
        return (sortBy ?? string.Empty).ToLowerInvariant() switch
        {
            "name" => descending ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name),
            "email" => descending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            _ => descending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt)
        };
    }
}

