using Preguntin_ASP.NET.Models.Usuarios;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Preguntin_ASP.NET.Models.Preguntas
{
    [Table("CategoriasUsuarios")]
    public class CategoriasUsuarios
    {
        [Key]
        public int Id { get; set; }

        public int IdCategoria { get; set; }

        [MaxLength(450)]
        public string IdUser { get; set; }

        [DefaultValue(0)]
        public int PuntosCategoria { get; set; }

    }
}
