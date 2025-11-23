using Mindly.Api.Domain.Exceptions;

namespace Mindly.Api.Domain.Entities;

public sealed class User
{
    private const int MinNameLength = 2;
    private const int MaxNameLength = 100;
    private const int MaxEmailLength = 255;

    private User()
    {
        // ORM
        FocusSessions = new List<FocusSession>();
    }

    public User(string name, string email)
    {
        Name = ValidateName(name);
        Email = ValidateEmail(email);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
        FocusSessions = new List<FocusSession>();
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Relacionamento: Um usuário pode ter várias sessões de foco
    public ICollection<FocusSession> FocusSessions { get; private set; }

    public void UpdateName(string name)
    {
        Name = ValidateName(name);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        Email = ValidateEmail(email);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string email)
    {
        Name = ValidateName(name);
        Email = ValidateEmail(email);
        UpdatedAt = DateTime.UtcNow;
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainValidationException("O nome é obrigatório.");
        }

        var trimmed = name.Trim();
        if (trimmed.Length < MinNameLength || trimmed.Length > MaxNameLength)
        {
            throw new DomainValidationException($"O nome deve ter entre {MinNameLength} e {MaxNameLength} caracteres.");
        }

        return trimmed;
    }

    private static string ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainValidationException("O email é obrigatório.");
        }

        var trimmed = email.Trim().ToLowerInvariant();
        if (trimmed.Length > MaxEmailLength)
        {
            throw new DomainValidationException($"O email pode ter no máximo {MaxEmailLength} caracteres.");
        }

        if (!trimmed.Contains('@') || !trimmed.Contains('.'))
        {
            throw new DomainValidationException("O email deve ter um formato válido.");
        }

        return trimmed;
    }
}

