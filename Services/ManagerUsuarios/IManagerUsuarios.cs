using Microsoft.AspNetCore.Identity;
using Preguntin_ASP.NET.Models.DTO;
using Preguntin_ASP.NET.Models.Usuarios;

namespace Preguntin_ASP.NET.Services.ManagerUsuarios
{
    public interface IManagerUsuarios
    {
        public Task<bool> RegisterJugadorAsync(Registro jugadorUser);
        public Task AddRoleUsuarioAsync(Jugador jugador, ICollection<string> roles);
        public Task<bool> verificacionCorreo(string email);
    }
}
