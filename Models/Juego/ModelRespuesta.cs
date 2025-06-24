using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Preguntin_ASP.NET.Models.Preguntas
{
    [Table("Respuestas")]
    public class ModelRespuesta
    {
        [Key]
        public int IdRespuesta { get; set; }
        [Required]
        public string texto { get; set; }
        public ModelPregunta? Pregunta { get; set; } //referencia de relacion
        public int IdPregunta { get; set; }  //clave foranea
        public bool isRespuestaCorrecta { get; set; }

    }
}
