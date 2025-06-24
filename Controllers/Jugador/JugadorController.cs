using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Preguntin_ASP.NET.Services.Juego;

namespace Preguntin_ASP.NET.Controllers.Jugador
{
    [Authorize(Policy ="Jugador")]
    [ApiController]
    [Route("/inf")]
    public class JugadorController : Controller
    {
        private readonly IJuegoService _juego;
        public JugadorController(IJuegoService juego)
        {
            _juego = juego;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
