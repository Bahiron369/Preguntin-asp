using AutoMapper;
using Preguntin_ASP.NET.Data;
using Preguntin_ASP.NET.GraphQL.Imputs;
using System.Security.Claims;
using Preguntin_ASP.NET.Models.Usuarios;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Preguntin_ASP.NET.Services.Token;
using Microsoft.IdentityModel.Tokens;
using System.Web;
using Preguntin_ASP.NET.Services;
using Preguntin_ASP.NET.Services.Informacion_Usuario;

namespace Preguntin_ASP.NET.GraphQL.Mutations
{
    
    public class Mutations
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        public Mutations(ApplicationDbContext dbContext, IMapper mapper)
        {
            _context = dbContext;
            _mapper = mapper;
        }

        [Authorize(Policy ="Jugador")]
        public async Task<JugadorPayLoad?> UpdateJugadorAsync(InputUpdateInfJugador inputJugador, ClaimsPrincipal claims,
            [Service] ITokenJwt token, [Service] IInfUsers infUsers)
        {
            try
            {
                string? idJugador = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Jugador jugador = new Jugador();
                try
                {
                    jugador = _context.jugadorUsers.Where(p => p.Id == idJugador).First();//encuentra al jugador con el ID
                }
                catch(Exception){
                    throw new Exception("No se encuentra el jugador");
                }
           
                if (idJugador == null) throw new Exception("No se encuentra el jugador"); //comprueba el nombre

                if (inputJugador.nombre != null)
                    await infUsers.UpdateNameAsync(idJugador, inputJugador.nombre);
               
                if (inputJugador.Email != null)
                    await infUsers.SendEmailAsync(idJugador, inputJugador.Email, jugador.Email); //metodo para actualizar el email
             
                if (inputJugador.newPassword != null && inputJugador.OldPassword != null) //metodo para actualizar la contraseña
                    await infUsers.UpdatePasswordAsync(idJugador, inputJugador.OldPassword, inputJugador.newPassword);

                if (inputJugador.telefono != null)
                    await infUsers.UpdateNumerorAsync(idJugador, inputJugador.telefono); //metodo paraactualizar el telefono
                
                _context.SaveChanges();

                return new JugadorPayLoad(token:await token.CreateTokenAsync(jugador),mensajes: "Se guardaron los cambios correctamente",true);

            }catch(Exception e) { 
                return new JugadorPayLoad(null, e.Message,false); 
            }
           
        }

        public async Task<string> ValidEmailForgetAsync([Service] IInfUsers infUsers, InputValidEmailForget inputUpdatePassword)
        {
            try{
                await infUsers.UpdatePasswordForgetAsync(inputUpdatePassword.email);
                return  "se envio el mensaje correctamente";
            }
            catch(Exception e){
                return e.Message;
            }
        }

        public async Task<bool> ValidEmailAsync(InputValidEmail validEmail, [Service] UserManager<Jugador> manager, [Service] IInfUsers infUsers)
        {
            try{
                Jugador jugador = _context.jugadorUsers.Where(p => p.Id == validEmail.id).First();//encuentra al jugador con el ID
                await infUsers.UptateEmailAsync(jugador,validEmail.newEmail,validEmail.tokenEmail);
                return true;
            }
            catch (Exception e){
                return false;
            }
        }

        public async Task<string> UpdatePasswordForgetAsync([Service] IInfUsers infUsers, InputUpdatePasswordForget inputUpdatePassword)
        {
            try{
                var result = await infUsers.CheckPasswordForgetAsync(inputUpdatePassword.email,inputUpdatePassword.token,inputUpdatePassword.password);
                if (!result.Succeeded) throw new Exception("Error la cambiar la contraseña");
                return "la contraseña se guardo correctamente";
            }
            catch (Exception e){
                return e.Message;
            }
        }

    }
}
