using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using Preguntin_ASP.NET.Data;
using Preguntin_ASP.NET.Models.DTO;
using Preguntin_ASP.NET.Models.Preguntas;
using Preguntin_ASP.NET.Services.Juego;

namespace Preguntin_ASP.NET.Hubs
{
    [Authorize(Policy = "Jugador")]
    public class HubPlay : Hub
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _profile;
        private readonly JuegoService _juegoService;
        public HubPlay(ApplicationDbContext dbContext, IMapper profile, JuegoService juegoService)
        {
            _context = dbContext;
            _profile = profile;
            _juegoService = juegoService;
        }

        public async Task Game(string idJugador)
        {
            await Clients.Caller.SendAsync("Categoria", _juegoService.ShowCategoria(idJugador));
        }
    }
}
