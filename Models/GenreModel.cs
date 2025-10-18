using System.ComponentModel.DataAnnotations;

namespace Gaby.io.Models;

public class GenreModel
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    // Relacionamento muitos-para-muitos com Book
    public ICollection<BookGenreModel> BookGenres { get; set; } = new List<BookGenreModel>();
}