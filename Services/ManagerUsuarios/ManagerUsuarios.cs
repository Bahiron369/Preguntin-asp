using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Preguntin_ASP.NET.Data;
using Preguntin_ASP.NET.Models.DTO;
using Preguntin_ASP.NET.Models.Juego;
using Preguntin_ASP.NET.Models.Usuarios;
using Preguntin_ASP.NET.Services.confirmacion_usuario;
using Preguntin_ASP.NET.Services.ManagerRoles;

namespace Preguntin_ASP.NET.Services.ManagerUsuarios
{
    public class ManagerUsuarios : IManagerUsuarios
    {
        private readonly UserManager<Jugador> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfirmInformation _information;
        private readonly ApplicationDbContext _context;
        private readonly RegistroAdmin _registroAdmin;
        public ManagerUsuarios(UserManager<Jugador> userManager, IMapper mapper, IConfirmInformation information,
                                ApplicationDbContext context, IOptions<RegistroAdmin> registroAdmin)
        {
            _userManager = userManager;
            _mapper = mapper;
            _information=information;
            _context = context;
            _registroAdmin = registroAdmin.Value;
        }

        private int TokenNumberConfirmEmail()
        {
            Random rnd = new Random();
            return rnd.Next(1000,9000);
        }

        private async Task<Guid> sendEmailAsync(int codigo,string email)
        {
            var confirEmail = new ConfirmarInDatos
            {
                Id = Guid.NewGuid(),
                Email = email,
                Token = codigo.ToString(),
                Expiracion = DateTime.UtcNow.AddDays(1)
            };

            await _information.SendMessageAsync(confirEmail.Email, confirEmail.Email, "Confirmacion de correo electronico", $"Codigo de verificacion: {codigo}");

            _context.confirmars.Add(confirEmail);
            _context.SaveChanges();

            return confirEmail.Id;
        }

        public async Task<bool> verificacionCorreo(string email)
        {
            try{
                await sendEmailAsync(TokenNumberConfirmEmail(), email);
                return true;
            }
            catch (Exception e){
                return false;
            }
        }

        //registro de usuarios
        public async Task<bool> RegisterJugadorAsync(Registro jugadorUser)
        {
            
            if(await _userManager.FindByNameAsync(jugadorUser.Name)!=null) throw new Exception("El Nombre de usuario ya esta registrado");
            if(await _userManager.FindByEmailAsync(jugadorUser.Email)!=null) throw new Exception("Este correo ya esta registrado");
            var verificar = await _context.confirmars.AnyAsync(p=>p.Email==jugadorUser.Email&&p.Token== jugadorUser.codigo.ToString() && DateTime.Now <= p.Expiracion);
            if (verificar)
            {
                var tokenTemporar = _context.confirmars.Where(p => p.Email == jugadorUser.Email).ToList();
                _context.confirmars.RemoveRange(tokenTemporar);
                await _context.SaveChangesAsync();

                Jugador jugador = new Jugador
                {
                    UserName = jugadorUser.Name,
                    Email = jugadorUser.Email,
                    PhoneNumber = jugadorUser.Telefono

                };//creamos un usuario

                //Registramos una contraseña
                if (jugadorUser.Password == null) throw new Exception("Sin Contraseña");
                else
                    if (await _userManager.CreateAsync(jugador, jugadorUser.Password) != IdentityResult.Success)
                    throw new Exception("Error al crear el usuario");

                 await AddRoleUsuarioAsync(jugador);

                return true;
            }
            else {
                throw new Exception("Codigo Invalido");
            }

        }

        //añadir el rol de jugador al usuario
        private async Task AddRoleUsuarioAsync(Jugador jugador)
        {
            if (_registroAdmin.Email.Contains(jugador.Email))
            {
                var result = await _userManager.AddToRolesAsync(jugador, _registroAdmin.Roles);
                if(result != IdentityResult.Success)
                    throw new Exception("Error al añadir el rol al usuario");
            }
            else if (await _userManager.AddToRoleAsync(jugador, "Jugador") != IdentityResult.Success)
                throw new Exception("Error al añadir el rol al usuario");
        }

        //añadir roles a un usuario
        public async Task AddRoleUsuarioAsync(Jugador jugador, ICollection<string> roles)
        {
            if (await _userManager.AddToRolesAsync(jugador, roles) != IdentityResult.Success)
                throw new Exception("Error al añadir el rol al usuario");
        }

      
         
       
    }
}
