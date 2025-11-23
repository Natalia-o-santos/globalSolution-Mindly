using System.ComponentModel.DataAnnotations;

namespace Mindly.Api.DTOs.Application;

public class UserUpdateDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório.")]
    [StringLength(255, ErrorMessage = "O email pode ter no máximo 255 caracteres.")]
    [EmailAddress(ErrorMessage = "O email deve ter um formato válido.")]
    public string Email { get; set; } = string.Empty;
}

