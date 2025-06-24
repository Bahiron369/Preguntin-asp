namespace Preguntin_ASP.NET.Models.Juego
{
    public class ConfirmarInDatos
    {
        public required Guid Id { get; set; }
        public required DateTime Expiracion { get; set; }
        public required string Token { get; set; }
        public required string Email { get; set; }

    }
}
