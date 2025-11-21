using Mindly.Api.Domain.Enums;
using Mindly.Api.Domain.Exceptions;

namespace Mindly.Api.Domain.Entities;

public sealed class FocusSession
{
    private const int MinFocusMinutes = 15;
    private const int MaxFocusMinutes = 150;
    private const int MinBreakMinutes = 5;
    private const int MaxBreakMinutes = 45;

    private FocusSession()
    {
        // ORM
    }

    public FocusSession(string title, int focusMinutes, int breakMinutes, string? description = null, bool enableIoT = true)
    {
        Title = title?.Trim() ?? throw new DomainValidationException("O título é obrigatório.");
        Description = description?.Trim();
        FocusMinutes = focusMinutes;
        BreakMinutes = breakMinutes;
        IoTIntegrationEnabled = enableIoT;
        ValidateRanges();
        Status = FocusSessionStatus.Planned;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int FocusMinutes { get; private set; }
    public int BreakMinutes { get; private set; }
    public FocusSessionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IoTIntegrationEnabled { get; private set; }

    public void UpdateDetails(string title, string? description, int focusMinutes, int breakMinutes, bool enableIoT)
    {
        Title = title?.Trim() ?? throw new DomainValidationException("O título é obrigatório.");
        Description = description?.Trim();
        FocusMinutes = focusMinutes;
        BreakMinutes = breakMinutes;
        IoTIntegrationEnabled = enableIoT;
        ValidateRanges();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Start()
    {
        if (Status == FocusSessionStatus.Completed)
        {
            throw new DomainValidationException("Não é possível reiniciar uma sessão concluída.");
        }

        Status = FocusSessionStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Pause()
    {
        if (Status != FocusSessionStatus.InProgress)
        {
            throw new DomainValidationException("Solo sessões em progresso podem ser pausadas.");
        }

        Status = FocusSessionStatus.Paused;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status == FocusSessionStatus.Completed)
        {
            return;
        }

        Status = FocusSessionStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    private void ValidateRanges()
    {
        if (FocusMinutes < MinFocusMinutes || FocusMinutes > MaxFocusMinutes)
        {
            throw new DomainValidationException($"O tempo de foco precisa estar entre {MinFocusMinutes} e {MaxFocusMinutes} minutos.");
        }

        if (BreakMinutes < MinBreakMinutes || BreakMinutes > MaxBreakMinutes)
        {
            throw new DomainValidationException($"A pausa precisa ter entre {MinBreakMinutes} e {MaxBreakMinutes} minutos.");
        }
    }
}
