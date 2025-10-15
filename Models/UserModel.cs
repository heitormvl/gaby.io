using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Gaby.io.Models
{
    public class UserModel : IdentityUser<string>
    {
        [Required, MaxLength(50)]
        public string DisplayName { get; set; } = string.Empty;

        public ICollection<ReadingModel> Readings { get; set; } = new List<ReadingModel>();
    }
}