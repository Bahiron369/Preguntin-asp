using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Preguntin_ASP.NET.Models.DTO;
using Preguntin_ASP.NET.Services.Juego;

namespace Preguntin_ASP.NET.Controllers.Juego
{
    [ApiController]
    [Authorize(Policy = "Jugador")]
    [Route("[Controller]/Player")]
    public class GameController : Controller
    {
        private readonly IJuegoService _juego;

        public GameController(IJuegoService juego)
        {
            _juego = juego;
        }

        [HttpGet("Categorias/{idJugador}")]
        public async Task<IActionResult> GetAllCategoriasAsync(string idJugador) => Ok(await _juego.ShowCategoria(idJugador));

        [HttpPut("PuntosCategoria/{idJugador}/{idCategoria}/{puntos}")]
        public async Task<IActionResult> SetPuntosCategoria(string idJugador, int idCategoria, int puntos) => Ok( await _juego.setPuntosCategoriasAsync(idJugador, idCategoria, puntos));

        [HttpGet("categoria/{nombreCategoria}")]
        public async Task<IActionResult> GetPreguntas(string nombreCategoria) => Ok(await _juego.GetQuestionAllAsync(nombreCategoria));

        [HttpGet("PuntosGlobalcategoria/top/{categoria}")]
        public async Task<IActionResult> GetPuntosGolbalCategoriaAsync(int categoria) => Ok(await _juego.GetPointGlobalCategoryAsync(categoria));

        [HttpPost("puntosJugador")]
        public async Task<IActionResult> SetPuntosJugador([FromBody] PuntosJugador jugadorPuntos) => Ok(await _juego.SetPuntosJugadorAsync(jugadorPuntos.Id, jugadorPuntos.Puntos));
    }
}
