using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Gaby.io.Models;

[Index(nameof(Name), IsUnique = true)]
public class GenreModel
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    // Relacionamento muitos-para-muitos com Book
    public ICollection<BookGenreModel> BookGenres { get; set; } = new List<BookGenreModel>();
}