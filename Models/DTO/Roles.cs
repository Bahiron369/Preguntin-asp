using System.ComponentModel.DataAnnotations;

namespace Preguntin_ASP.NET.Models.DTO
{
    public class Roles
    {
        [MinLength(1)]
        public required ICollection<string> Name { get; set; }

        public ICollection<string>? NewName { get; set; }
    }
}
