using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gaby.io.Models;

public class BookGenreModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BookId { get; set; }

    [Required]
    public int GenreId { get; set; }

    [ForeignKey(nameof(BookId))]
    public BookModel Book { get; set; } = null!;

    [ForeignKey(nameof(GenreId))]
    public GenreModel Genre { get; set; } = null!;
}
