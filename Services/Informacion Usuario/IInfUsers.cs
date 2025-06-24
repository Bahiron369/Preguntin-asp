using Microsoft.AspNetCore.Identity;
using Preguntin_ASP.NET.Models.Usuarios;

namespace Preguntin_ASP.NET.Services.Informacion_Usuario
{
    public interface IInfUsers
    {
        public Task SendEmailAsync(string id, string email, string currentEmail);
        public Task<IdentityResult> UpdateNameAsync(string id, string name);
        public Task UpdateNumerorAsync(string id, string numero);
        public Task<IdentityResult> UpdatePasswordAsync(string id, string oldPassword, string newPassword);
        public Task<IdentityResult> CheckPasswordForgetAsync(string email, string tokenPassword, string newPassword);
        public Task<bool> CheckEmailJugador(string email);
        public Task<bool> CheckNombreJugador(string name);
        public Task<bool> CheckNumberJugador(string numero);
        public Task UpdatePasswordForgetAsync(string email);
        public Task UptateEmailAsync(Jugador jugador, string newEmail, string tokenEmail);
    }
}
