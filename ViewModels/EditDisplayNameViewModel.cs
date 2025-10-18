using System.ComponentModel.DataAnnotations;

namespace Gaby.io.ViewModels;

public class EditDisplayNameViewModel
{
    [Required(ErrorMessage = "O nome de exibição é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome de exibição pode ter no máximo 100 caracteres.")]
    public required string DisplayName { get; set; }
}