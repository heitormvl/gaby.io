using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gaby.io.Models;

public class ReadingModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BookId { get; set; }

    [Required]
    public required string UserId { get; set; }

    public int? Year { get; set; }

    [Range(1, 12)]
    public int? Month { get; set; }

    [Column(TypeName = "decimal(2,1)")]
    public decimal? Rating { get; set; }

    [ForeignKey(nameof(BookId))]
    public BookModel Book { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; set; } = null!;
}