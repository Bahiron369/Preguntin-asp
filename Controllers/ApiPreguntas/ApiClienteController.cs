using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Preguntin_ASP.NET.Services.HttpCliente;

namespace Preguntin_ASP.NET.Controllers.ApiPreguntas
{
    [Route("ApiCliente")]
    public class ApiClienteController : Controller
    {
        private readonly IApiHttpService _apiHttpService;
        public ApiClienteController(IApiHttpService apiHttpService) {
            _apiHttpService = apiHttpService;
        }

        [HttpGet]
        [Authorize(Policy = "Jugador")]
        public async Task<IActionResult> Index() =>  Ok(await _apiHttpService.GetPreguntaAsync("Entertainment: Film"));

        [HttpGet("Public")]
        public async Task<IActionResult> GetPreguntas() => Ok(await _apiHttpService.GetPreguntaAsync("Public"));
        
    }
}
