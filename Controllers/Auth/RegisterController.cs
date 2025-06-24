using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Preguntin_ASP.NET.Models.DTO;
using Preguntin_ASP.NET.Models.Usuarios;
using Preguntin_ASP.NET.Services.Informacion_Usuario;
using Preguntin_ASP.NET.Services.ManagerUsuarios;

namespace Preguntin_ASP.NET.Controllers.Auth
{
    [Route("Account/[Controller]")]
    public class RegisterController : Controller
    {
        private readonly IManagerUsuarios _managerUsuarios;
        private readonly IInfUsers _checkInf;
        public RegisterController(IManagerUsuarios managerUsuarios, IInfUsers checkInf)
        {
            _managerUsuarios = managerUsuarios;
            _checkInf = checkInf;
        }

        /// <summary>
        /// Este controlador sirve para conbrovar que no exista informacion en la base de datos, mantiene la autenticidad y evita duplicados
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("check")]
        public async Task<IActionResult> CheckInformacionRegistro([FromBody] Registro user){
            Dictionary<string, bool> checkInformacion = new Dictionary<string, bool>()
            {
               { "nombre", await _checkInf.CheckNombreJugador(user.Name)},
               { "email", await _checkInf.CheckEmailJugador(user.Email) },
               {"numero", await _checkInf.CheckNumberJugador(user.Telefono)}
            };

            return Ok(checkInformacion);
        }


        [HttpPost]
        public async Task<IActionResult> Index([FromBody] Registro user)
        {
            try{
                return Ok(await _managerUsuarios.RegisterJugadorAsync(user));
            }
            catch (Exception ex) { return BadRequest(ex.Message); }

        }

        [HttpPost("valid")]
        public async Task<IActionResult> ValidarEmail([FromBody] Email correo)
        {
            try{
                return Ok(await _managerUsuarios.verificacionCorreo(correo.email));
            }
            catch (Exception ex) { return BadRequest(ex.Message); }

        }

    }
}
