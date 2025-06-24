using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Preguntin_ASP.NET.Data;
using Preguntin_ASP.NET.Models.DTO;
using Preguntin_ASP.NET.Models.Usuarios;
using Preguntin_ASP.NET.Services.Juego;
using Preguntin_ASP.NET.Services.ManagerRoles;

namespace Preguntin_ASP.NET.Services.Admin
{
    public partial class AdminService
    {
        private readonly UserManager<Jugador> _userManager;
        private readonly IRolesUsers _rol;
        private readonly IJuegoService _juego;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public AdminService(UserManager<Jugador> userManager, IRolesUsers rol, IJuegoService juego, ApplicationDbContext context, IMapper mapper) {
            _userManager = userManager;
            _rol = rol;
            _juego = juego;
            _context = context;
            _mapper = mapper;
        }

        //crear usuario por defecto 
        //mostrar todos los usuarios
        public async Task<ICollection<User>>? GetAllUsers()
        {
            //agremasmos los valores del modelo a mostrar
            List<User> jugadores = _userManager.Users.Select(propiedad => new User
            {
                Name = propiedad.UserName,
                Email = propiedad.Email,
                Telefono = propiedad.PhoneNumber,
                Id = propiedad.Id,
            }
             ).ToList();

            if (jugadores == null)
                throw new Exception("No hay usuarios Registrados");

            return await ObtenerRole(jugadores);
        }

        public async Task<ICollection<User>> ObtenerRole(ICollection<User> jugadores) { 
            //agregamos roles
            foreach (User user in jugadores) {
                Jugador? usuario = await _userManager.FindByIdAsync(user.Id);//obtenemos usuario
                if(usuario!=null)
                    user.Rol = await _userManager.GetRolesAsync(usuario);//agregamos roles
            }
            return jugadores;
        }

        //obtener informacion un solo usuario 
        public async Task<User>? GetUserAsync(string Id)
        {
            Jugador? usuario = await _userManager.FindByIdAsync(Id);///obtenemos un usuario

            if (usuario == null)
                throw new Exception("No existe el usuario");

            var InfUsuario =  new User //obtenemos los datos del usuario
            {
                Name = usuario.UserName,
                Email = usuario.Email,
                Telefono = usuario.PhoneNumber,
                Id = usuario.Id,
            };

            InfUsuario.Rol = await _userManager.GetRolesAsync(usuario);
            return InfUsuario;
        }


        //eliminar jugador con Id
        public async Task DeleteUserIdAsync(string Id) => await ResultDelete(await _userManager.FindByIdAsync(Id));

        //EliminarJugador por Nombre
        //resultado de eliminar a un jugador
        private async Task ResultDelete(Jugador jugador)
        {
            if (jugador != null)
                await _userManager.DeleteAsync(jugador);
            else
                throw new Exception("Usuario no encontrado");
        }

        //alterar rol del usuario
        public async Task<bool> UpdateRole(string id, string[] roles)
        {
            Jugador? jugador = await _userManager.FindByIdAsync(id);    
            
            if (jugador != null && roles!=null && roles.Length>0)
            {
                if (await _rol.ComprobarRoleExist(roles))//comprueba que los roles a insertar sean validos
                {
                    //eliminamos los roles que tiene
                    if (await _userManager.RemoveFromRolesAsync(jugador, await _userManager.GetRolesAsync(jugador)) == IdentityResult.Success)
                    {
                        if (await _userManager.AddToRolesAsync(jugador, roles) == IdentityResult.Success) //agregamos los nuevos
                            return true;
                        else throw new Exception("Error al añadir usuario");
                    }
                    else throw new Exception("Error al eliminar los roles existentes"); 
                }
                else throw new Exception("No existe los roles ingresados"); 
            }
            else throw new Exception("Usuario o rol nulo");//por si falla
        }


    }
}
