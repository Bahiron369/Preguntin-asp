using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders;
using Microsoft.IdentityModel.Tokens;
using Preguntin_ASP.NET.Data;
using Preguntin_ASP.NET.Models.Juego;
using Preguntin_ASP.NET.Models.Usuarios;
using Preguntin_ASP.NET.Services.confirmacion_usuario;
using Preguntin_ASP.NET.Services.Informacion_Usuario;
using System.Text;

namespace Preguntin_ASP.NET.Services.Juego
{

    public class InfUsers : IInfUsers
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<Jugador> _manager;
        private readonly IConfirmInformation _confirm;

        public InfUsers(IMapper mapper, ApplicationDbContext dbContext, UserManager<Jugador> manager, IConfirmInformation confirm)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _confirm = confirm;
            _manager = manager;
        }

        //Obtiene el usuario por el id
        private Jugador GetJugadorById(string id)
        {

            Jugador? jugador = _dbContext.jugadorUsers.Where(p => p.Id == id).First();
            if (jugador == null)
                throw new Exception("Jugador no encontrado");
            return jugador;
        }

        /// <summary>
        /// Actualiza el nombre y chechea que no haya nadie mas con este nombre
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<IdentityResult> UpdateNameAsync(string id, string name)
        {
            var jugador = GetJugadorById(id);

            //valida que el nombre del usuario no exista en la base de datos
            if (!await CheckNombreJugador(name))
                jugador.UserName = name;
            else
                throw new Exception("El nombre del usuario ya existe");

            return await _manager.UpdateAsync(jugador);
        }
        public async Task<bool> CheckNombreJugador(string name) => await _dbContext.jugadorUsers.AnyAsync(p => p.UserName == name);

        /// <summary>
        /// Comprobacion completa del cambio de email
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task SendEmailAsync(string id, string email, string currentEmail)
        {
            var jugador = GetJugadorById(id);

            if (!await CheckEmailJugador(email))
            {
                string tokenEmail = await _manager.GenerateChangeEmailTokenAsync(jugador, email); //obtener token
                string UrlToken = EncodificarTokenEmail(tokenEmail); //decodificar
                string UrlEmail = EncodificarTokenEmail(email);
                string ConfirmTokenEmail = $"Para confirmar el email: <a href=\"http://localhost:4200/dashboard/informacion?email={UrlEmail}&token={UrlToken}\">Has click aqui<a>";
                await _confirm.SendMessageAsync(currentEmail, email, "CONFIRMACION DE CORREO PREGUNTIN", ConfirmTokenEmail);
            }
            else
                throw new Exception("El email del usuario ya existe");
        }

        public async Task UptateEmailAsync(Jugador jugador, string newEmail, string tokenEmail)
        {
            var tokenDecodificado = DecodificarTokenEmail(tokenEmail);
            var emailDecodificado = DecodificarTokenEmail(newEmail);
            var result = await _manager.ChangeEmailAsync(jugador, emailDecodificado, tokenDecodificado);
            if (!result.Succeeded) throw new Exception("Error la cambiar el email");
        }

        private string EncodificarTokenEmail(string tokenEmail) => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(tokenEmail)); //decodificar

        public string DecodificarTokenEmail(string token) => Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

        public async Task<bool> CheckEmailJugador(string email) => await _dbContext.jugadorUsers.AnyAsync(p => p.Email == email);

        /// <summary>
        /// Paso para la confirmacion de numeros
        /// </summary>
        /// <param name="id"></param>
        /// <param name="numero"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task UpdateNumerorAsync(string id, string numero)
        {
            var jugador = GetJugadorById(id);

            if (!await CheckNumberJugador(numero) || numero == "" || numero == null)
            {
                jugador.PhoneNumber = numero;
                await _manager.UpdateAsync(jugador);
            }
            else
                throw new Exception("El numero del usuario ya existe");
        }
        public async Task<bool> CheckNumberJugador(string numero) => await _dbContext.jugadorUsers.AnyAsync(p => p.PhoneNumber == numero);

        /// <summary>
        /// actualiza la contraseña del usuario 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="numero"></param>
        /// <returns></returns>
        public async Task<IdentityResult> UpdatePasswordAsync(string id, string oldPassword, string newPassword)
        {
            var jugador = GetJugadorById(id); //busca jugador por id
            bool VerificacionCurrentPassword = await _manager.CheckPasswordAsync(jugador, oldPassword);//verifica que la contraseña actual sea la correcta
            if (!VerificacionCurrentPassword) throw new Exception("La contrase actual no es correcta");//si no lo es devuelve un error
            return await _manager.ChangePasswordAsync(jugador, oldPassword, newPassword); //si la contraseña es correcta la guarda con la nueva
        }

        public async Task UpdatePasswordForgetAsync(string email)
        {
            Jugador? jugador = await GetJugadorByEmailAsync(email);

            string? tokenPassword = await _manager.GeneratePasswordResetTokenAsync(jugador);

            var datos = new ConfirmarInDatos
            {
                Id = Guid.NewGuid(),
                Token = tokenPassword,
                Expiracion = DateTime.UtcNow.AddDays(1),
                Email = email,
            };

            _dbContext.confirmars.Add(datos);
            await _dbContext.SaveChangesAsync();

            if (tokenPassword == null) throw new Exception("error al cambiar la contraseña");
            string UrlToken = EncodificarTokenEmail(tokenPassword); //decodificar
            string UrlEmail = EncodificarTokenEmail(email);
            await _confirm.SendMessageAsync(email, email, "Restauración de contraseña", $"<a href='http://localhost:4200/auth/login/forget/?token={UrlToken}&send=true&email={UrlEmail}'>Has click aqui para cambio de contraseña</a>");
        }

        public async Task<IdentityResult> CheckPasswordForgetAsync(string emailCodificado, string tokenPassword, string newPassword)
        {
            var email = DecodificarTokenEmail(emailCodificado);
            var tokenDecodificado = DecodificarTokenEmail(tokenPassword);
            Jugador? jugador = await GetJugadorByEmailAsync(email);
            ConfirmarInDatos? comprobar = _dbContext.confirmars.Where(p => p.Token == tokenDecodificado && p.Expiracion >= DateTime.Now).First();

            if (comprobar != null)
            {
                PasswordHasher<string> _passwordHasher = new PasswordHasher<string>();
                jugador.PasswordHash = _passwordHasher.HashPassword(null, newPassword);
                _dbContext.confirmars.Remove(comprobar);
                _dbContext.Update(jugador);
                await _dbContext.SaveChangesAsync();
                return IdentityResult.Success;
            }

            throw new Exception("Error al cambiar la contraseña");
        }

        private async Task<Jugador?> GetJugadorByEmailAsync(string email)
        {
            Jugador? jugador = await _manager.FindByEmailAsync(email);
            if (jugador == null) throw new Exception("Email no encontrado");
            return jugador;
        }
    }
}
