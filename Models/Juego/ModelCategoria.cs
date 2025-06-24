using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Preguntin_ASP.NET.Models.Preguntas
{
    [Table("Categorias")]
    public class ModelCategoria
    {
        [Key]
        public int IdCategoria { get; set; }

        [Required, MaxLength(50)]
        public string? Nombre { get; set; }

        [Required]
        public int PuntosCategoria { get; set; } = 0;

        //relaciones
        public ICollection<ModelPregunta>? preguntas { get; set; } 
    }
}
