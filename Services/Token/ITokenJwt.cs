using Preguntin_ASP.NET.Models.Usuarios;
using System.Security.Claims;

namespace Preguntin_ASP.NET.Services.Token
{
    public interface ITokenJwt
    {
        public Task<string> CreateTokenAsync(Jugador user);

    }
}
