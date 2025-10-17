using System.ComponentModel.DataAnnotations;

namespace Gaby.io.ViewModels;

public class PublisherFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O nome pode ter no máximo 100 caracteres.")]
    public string Name { get; set; } = string.Empty;
}
