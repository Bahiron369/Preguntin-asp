using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Preguntin_ASP.NET.Models.DTO
{
    public class CategoriaDTO
    {
        public int? IdCategoria { get; set; }
        public string? Nombre { get; set; }

        public int PuntosCategoria { get; set; }
    }
}
