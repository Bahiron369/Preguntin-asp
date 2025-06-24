using Microsoft.AspNetCore.Identity;
using Preguntin_ASP.NET.Models.Preguntas;

namespace Preguntin_ASP.NET.Models.Usuarios
{
    public class Jugador : IdentityUser
    {
        public int Puntos { get; set; } 

    }
}
