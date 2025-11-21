using System.Linq;
using Microsoft.EntityFrameworkCore;
using Mindly.Api.Data;
using Mindly.Api.Domain.Entities;
using Mindly.Api.DTOs.Application;

namespace Mindly.Api.Repositories;

public class FocusSessionRepository : IFocusSessionRepository
{
    private readonly ApplicationDbContext _context;

    public FocusSessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<FocusSession>> SearchAsync(FocusSessionQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var normalizedPage = Math.Max(parameters.Page, 1);
        var normalizedPageSize = Math.Clamp(parameters.PageSize, 5, 100);
        var query = _context.FocusSessions.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Title))
        {
            var title = parameters.Title.Trim().ToLowerInvariant();
            query = query.Where(s => s.Title.ToLower().Contains(title));
        }

        if (parameters.Status.HasValue)
        {
            query = query.Where(s => s.Status == parameters.Status.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        query = ApplySorting(query, parameters.SortBy, parameters.Descending);

        var items = await query
            .Skip((normalizedPage - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<FocusSession>(items, normalizedPage, normalizedPageSize, total);
    }

    public async Task<FocusSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.FocusSessions.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task AddAsync(FocusSession session, CancellationToken cancellationToken = default)
    {
        await _context.FocusSessions.AddAsync(session, cancellationToken);
    }

    public void Update(FocusSession session)
    {
        _context.FocusSessions.Update(session);
    }

    public void Remove(FocusSession session)
    {
        _context.FocusSessions.Remove(session);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<FocusSession> ApplySorting(IQueryable<FocusSession> query, string? sortBy, bool descending)
    {
        return (sortBy ?? string.Empty).ToLowerInvariant() switch
        {
            "focusminutes" => descending ? query.OrderByDescending(s => s.FocusMinutes) : query.OrderBy(s => s.FocusMinutes),
            "breakminutes" => descending ? query.OrderByDescending(s => s.BreakMinutes) : query.OrderBy(s => s.BreakMinutes),
            "status" => descending ? query.OrderByDescending(s => s.Status) : query.OrderBy(s => s.Status),
            _ => descending ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt)
        };
    }
}
