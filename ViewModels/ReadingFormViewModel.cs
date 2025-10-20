using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gaby.io.ViewModels;

public class ReadingFormViewModel
{
    public int? Id { get; set; }

    [Display(Name = "Livro")]
    [Required(ErrorMessage = "Selecione um livro.")]
    public int BookId { get; set; }

    [Display(Name = "Data de início")]
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "A data de início é obrigatória.")]
    public DateTime StartDate { get; set; }

    [Display(Name = "Data de término")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    [Required(ErrorMessage = "Informe o status da leitura.")]
    [MaxLength(20)]
    public string Status { get; set; } = string.Empty;

    [Display(Name = "Páginas lidas")]
    [Range(0, 10000, ErrorMessage = "Informe um número válido de páginas.")]
    public int PagesRead { get; set; }

    [Display(Name = "Avaliação")]
    [Range(0, 5, ErrorMessage = "A avaliação deve ser entre 0 e 5.")]
    public int? Rating { get; set; }

    // Lista de livros com informações completas
    public List<BookSelectViewModel> AvailableBooks { get; set; } = new();

    // SelectListItems para compatibilidade com asp-items
    public IEnumerable<SelectListItem>? Books => AvailableBooks?
        .Select(b => new SelectListItem 
        { 
            Value = b.Id.ToString(), 
            Text = b.Title 
        });
}
