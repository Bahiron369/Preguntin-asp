using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Preguntin_ASP.NET.Models.Preguntas
{
    [Table("Preguntas")]
    public class ModelPregunta
    {
        [Key]
        public int IdPregunta { get; set; }

        [Required,MaxLength(150)]
        public string? Nombre { get; set; }

        [Required]
        public string dificultad { get; set; }

        [Required]
        public string Tipo { get; set; }

        [Required]
        public int PuntoPregunta { get; set; } = 0;

        [Required]
        public TimeSpan TiempoRespuesta { get; set; } =  new TimeSpan(00,01,30);
        public int IdCategoria { get; set; }
        public ICollection<ModelRespuesta>? respuesta { get; set; }
        public ModelCategoria? Categoria { get; set; }  


    }
}
