using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gaby.io.ViewModels;

public class BookFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    [MaxLength(200, ErrorMessage = "O título pode ter no máximo 200 caracteres.")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Autor")]
    [Required(ErrorMessage = "O autor é obrigatório.")]
    public int AuthorId { get; set; }

    [Display(Name = "Editora")]
    public int? PublisherId { get; set; }

    [Display(Name = "Gênero")]
    public int? GenreId { get; set; }

    [Display(Name = "Número de páginas")]
    [Range(1, 10000, ErrorMessage = "Insira um número válido de páginas.")]
    public int PageCount { get; set; }

    [Display(Name = "Data de publicação")]
    [DataType(DataType.Date)]
    public DateTime? PublicationDate { get; set; }

    // Dropdowns populados no controller
    public IEnumerable<SelectListItem>? Authors { get; set; }
    public IEnumerable<SelectListItem>? Publishers { get; set; }
    public IEnumerable<SelectListItem>? Genres { get; set; }
}
