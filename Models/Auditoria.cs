using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Preguntin_ASP.NET.Models
{
    public class Auditoria
    {
        [Key]
        public int IdAuditoria { get; set; }

		[Required, MaxLength(450)]
        public string IdUser { get; set; }

		[Required]
		public DateTime Fecha { get; set; }

		[Required,MaxLength(10)]
		public string Metodo { get; set; }

		[Required]
		public string IpUser { get; set; }

		[Required,MaxLength(50)]
		public string NameTable { get; set; }	

		[Required, MaxLength(50)]
		public string NameCampo { get; set; }

		[Required, MaxLength(450)]
        public string NuevoValor { get; set; }

        [Required, MaxLength(450)]
        public string ViejoValor { get; set; }

    }
}
