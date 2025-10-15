using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gaby.io.Models;

public class AuthorModel
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int? CountryId { get; set; }

    [Required, Column(TypeName = "char(1)")]
    public char Gender { get; set; }

    [ForeignKey(nameof(CountryId))] public CountryModel? Country { get; set; }
    public ICollection<BookModel> Books { get; set; } = new List<BookModel>();
}