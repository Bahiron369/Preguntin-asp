using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;

namespace Preguntin_ASP.NET.Models.DTO
{
    public class User
    {
        public string? Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Telefono { get; set; }
        public ICollection<string>? Rol { get; set; }
    }

    public class PuntosJugador
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public required int Puntos { get; set; }
    }
}
