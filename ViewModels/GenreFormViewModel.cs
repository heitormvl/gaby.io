using System.ComponentModel.DataAnnotations;

namespace Gaby.io.ViewModels;

public class GenreFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(50, ErrorMessage = "O nome pode ter no máximo 50 caracteres.")]
    public string Name { get; set; } = string.Empty;
}
