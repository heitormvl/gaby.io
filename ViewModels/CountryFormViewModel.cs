using System.ComponentModel.DataAnnotations;

namespace Gaby.io.ViewModels;

public class CountryFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(50, ErrorMessage = "O nome pode ter no máximo 50 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "O código é obrigatório.")]
    [StringLength(3, MinimumLength = 2, ErrorMessage = "O código deve ter 2 ou 3 caracteres.")]
    public string Code { get; set; } = string.Empty;
}