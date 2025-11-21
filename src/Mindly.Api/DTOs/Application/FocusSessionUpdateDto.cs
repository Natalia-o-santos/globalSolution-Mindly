using System.ComponentModel.DataAnnotations;
using Mindly.Api.Domain.Enums;

namespace Mindly.Api.DTOs.Application;

public class FocusSessionUpdateDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(150, ErrorMessage = "O título pode ter até 150 caracteres.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(400, ErrorMessage = "A descrição pode ter até 400 caracteres.")]
    public string? Description { get; set; }

    [Range(15, 150, ErrorMessage = "O tempo de foco deve ficar entre 15 e 150 minutos.")]
    public int FocusMinutes { get; set; }

    [Range(5, 45, ErrorMessage = "A pausa precisa ser entre 5 e 45 minutos.")]
    public int BreakMinutes { get; set; }

    public bool IoTIntegrationEnabled { get; set; } = true;
    public FocusSessionStatus? Status { get; set; }
}
