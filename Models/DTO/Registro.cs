using System.ComponentModel.DataAnnotations;

namespace Preguntin_ASP.NET.Models.DTO
{
    public class Registro
    {
        public required string Name { get; set; }
        public required string Email { get; set; }

        public string? Password { get; set; }

        [MinLength(10)]
        public string? Telefono { get; set; }
        public required int codigo { get; set; }
    }

    public class Email
    {
        public string email { get; set; }
    }
}
