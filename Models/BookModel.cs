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
    
    [Range(1, 10000)]
    public int PageCount { get; set; }

    [DataType(DataType.Date)]
    public DateTime? PublicationDate { get; set; }

    [ForeignKey(nameof(AuthorId))]
    public AuthorModel Author { get; set; } = null!;

    [ForeignKey(nameof(PublisherId))]
    public PublisherModel? Publisher { get; set; }

    // Relacionamento muitos-para-muitos com Genre
    public ICollection<BookGenreModel> BookGenres { get; set; } = new List<BookGenreModel>();
    public ICollection<ReadingModel> Readings { get; set; } = new List<ReadingModel>();
}