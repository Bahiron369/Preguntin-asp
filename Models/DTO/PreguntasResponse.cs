using Preguntin_ASP.NET.Models.Preguntas;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Preguntin_ASP.NET.Models.DTO
{
    public class PreguntasResponse
    { 
        public int? Id { get; set; }
        public string? Nombre { get; set; }

        public string dificultad { get; set; }

        public string Tipo { get; set; }

        public int PuntoPregunta { get; set; }

        public TimeSpan TiempoRespuesta { get; set; }
        public string respuestasCorrecta { get; set; }
        public ICollection<string>? respuestasIncorrecta { get; set; }
        public string NombreCategoria { get; set; }
    }
}
