using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gaby.io.Models;

public class BookModel
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public int AuthorId { get; set; }

    public int? PublisherId { get; set; }
    public int? GenreId { get; set; }
    public int? PageCount { get; set; }

    [ForeignKey(nameof(AuthorId))]
    public AuthorModel Author { get; set; } = null!;

    [ForeignKey(nameof(PublisherId))]
    public PublisherModel? Publisher { get; set; }

    [ForeignKey(nameof(GenreId))]
    public GenreModel? Genre { get; set; }

    public ICollection<ReadingModel> Readings { get; set; } = new List<ReadingModel>();
}