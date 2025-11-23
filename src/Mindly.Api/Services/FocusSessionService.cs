using Mindly.Api.Domain.Entities;
using Mindly.Api.Domain.Enums;
using Mindly.Api.Domain.Exceptions;
using Mindly.Api.DTOs.Application;
using Mindly.Api.Repositories;

namespace Mindly.Api.Services;

public class FocusSessionService : IFocusSessionService
{
    private readonly IFocusSessionRepository _repository;

    public FocusSessionService(IFocusSessionRepository repository)
    {
        _repository = repository;
    }

    public async Task<FocusSession> CreateAsync(FocusSessionCreateDto dto, CancellationToken cancellationToken = default)
    {
        var session = new FocusSession(dto.Title, dto.FocusMinutes, dto.BreakMinutes, dto.UserId, dto.Description, dto.IoTIntegrationEnabled);
        await _repository.AddAsync(session, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return session;
    }

    public async Task<FocusSession> UpdateAsync(Guid id, FocusSessionUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var session = await _repository.GetByIdAsync(id, cancellationToken);

        if (session is null)
        {
            throw new DomainValidationException("sessão de foco não encontrada.");
        }

        session.UpdateDetails(dto.Title, dto.Description, dto.FocusMinutes, dto.BreakMinutes, dto.IoTIntegrationEnabled);

        if (dto.Status.HasValue)
        {
            UpdateStatus(session, dto.Status.Value);
        }

        _repository.Update(session);
        await _repository.SaveChangesAsync(cancellationToken);
        return session;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var session = await _repository.GetByIdAsync(id, cancellationToken);

        if (session is null)
        {
            throw new DomainValidationException("sessão de foco não encontrada.");
        }

        _repository.Remove(session);
        await _repository.SaveChangesAsync(cancellationToken);
    }

    public async Task<FocusSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<PagedResult<FocusSession>> SearchAsync(FocusSessionQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        return await _repository.SearchAsync(parameters, cancellationToken);
    }

    private static void UpdateStatus(FocusSession session, FocusSessionStatus requestedStatus)
    {
        switch (requestedStatus)
        {
            case FocusSessionStatus.Planned:
                // When changing back to planned, just update timestamps
                session.UpdateDetails(session.Title, session.Description, session.FocusMinutes, session.BreakMinutes, session.IoTIntegrationEnabled);
                break;
            case FocusSessionStatus.InProgress:
                session.Start();
                break;
            case FocusSessionStatus.Paused:
                session.Pause();
                break;
            case FocusSessionStatus.Completed:
                session.Complete();
                break;
        }
    }
}
