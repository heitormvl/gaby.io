using System.ComponentModel.DataAnnotations;


namespace Gaby.io.Models;

public class CountryModel
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(3)]
    public string Code { get; set; } = string.Empty;

    public ICollection<AuthorModel> Authors { get; set; } = new List<AuthorModel>();
}