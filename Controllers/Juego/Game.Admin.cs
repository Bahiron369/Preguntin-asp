using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Preguntin_ASP.NET.Models.DTO;
using Preguntin_ASP.NET.Services.Admin;
using Preguntin_ASP.NET.Services.Juego;

namespace Preguntin_ASP.NET.Controllers.Juego
{
    [ApiController]
    [Route("Game/Admin/[Controller]")]
    public class PreguntasController : Controller
    {
        private readonly AdminService _adminService;
        private readonly IJuegoService _juego;
        public PreguntasController(AdminService adminService, IJuegoService juego)
        {
            _adminService = adminService;
            _juego = juego;
        }

        [Authorize(Policy ="Admin")]
        [HttpPost("{nombreCategoria}")]
        public async Task<IActionResult> setPreguntas([FromBody] List<PreguntasResponse> preguntas,string nombreCategoria)
        {
            var mensaje = await _adminService.setPreguntas(preguntas, nombreCategoria);
            return Ok(new { mensaje });
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete("delete/category/{idCategoria}")]
        public async Task<IActionResult> DeleteCategory(int idCategoria) => Ok(await _adminService.DeleteCategoria(idCategoria));

        [Authorize(Policy = "Admin")]
        [HttpDelete("delete/pregunta/{idPregunta}")]
        public async Task<IActionResult> DeleteQuestion(int idPregunta) => Ok(await _adminService.DeleteQuestion(idPregunta));


        [Authorize(Policy = "Admin")]
        [HttpGet("category/{idCategoria}/{nombre}/preguntas")]
        public async Task<IActionResult> getAllQuestion(int idCategoria, string nombre) => Ok(_juego.GetQuestionsDb(idCategoria,nombre));

        [Authorize(Policy = "Admin")]
        [HttpDelete("{idCategoria}")]
        public async Task<IActionResult> DeleteAllQuestion(int idCategoria) => Ok(await _adminService.deleteQuestionAllAsync(idCategoria));
    }
}
