using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gaby.io.ViewModels;

public class AuthorFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O nome pode ter no máximo 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "O gênero é obrigatório.")]
    [RegularExpression("^[MFNBmfnb]$", ErrorMessage = "O gênero deve ser M, F ou N (não-binário).")]
    public char Gender { get; set; }

    [Display(Name = "País de origem")]
    public int? CountryId { get; set; }

    // Populado pelo controller (para o dropdown)
    public IEnumerable<SelectListItem>? Countries { get; set; }
}
