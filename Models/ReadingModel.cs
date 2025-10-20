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

    [Range(0, 5)]
    public int? Rating { get; set; }

    // Novos campos
    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Em progresso";

    [Range(0, 10000)]
    public int PagesRead { get; set; }

    [ForeignKey(nameof(BookId))]
    public BookModel Book { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; set; } = null!;
}