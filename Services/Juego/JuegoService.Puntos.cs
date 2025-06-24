using AutoMapper;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Preguntin_ASP.NET.Data;
using Preguntin_ASP.NET.Models.DTO;
using Preguntin_ASP.NET.Models.Usuarios;
using Preguntin_ASP.NET.Services.HttpCliente;

namespace Preguntin_ASP.NET.Services.Juego
{
    public partial class JuegoService : IJuegoService
    {
        private readonly UserManager<Jugador> _usuariosManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IApiHttpService _apiHttpService;
     

        public JuegoService(UserManager<Jugador> usuariosMAnager, ApplicationDbContext context, IMapper mapper, IApiHttpService apiHttpService) { 

            _usuariosManager = usuariosMAnager;
            _context = context;
            _mapper = mapper;
            _apiHttpService = apiHttpService;
        }

         public async Task<List<PuntosJugador>> GetPointGlobalAsync()
         {
            //obtiene los primeros 10 jugadores con el puntaje mas alto
            List<Jugador>? jugadores = _usuariosManager.Users.OrderByDescending(p=>p.Puntos).Take(10).ToList();
            return _mapper.Map<List<PuntosJugador>>(jugadores); //retorna los jugadores
         }

        public async Task<List<PuntosJugador>> GetPointGlobalCategoryAsync(int idCategory)
        {
            var puntosCategoria = _context.categoriasUsuarios.Where(p=>p.IdCategoria==idCategory).OrderByDescending(p => p.PuntosCategoria).Take(10).ToList();
            List<PuntosJugador> jugadores = new List<PuntosJugador>();

            foreach (var categoria in puntosCategoria)
            {
                try
                {
                    var jugador = await _usuariosManager.FindByIdAsync(categoria.IdUser);
                    if(jugador == null)
                    {
                        _context.categoriasUsuarios.Remove(categoria);
                        _context.SaveChanges();
                    }else
                    {
                        var jugadorPuntos = new PuntosJugador()
                        {
                            Name = jugador.UserName,
                            Puntos = categoria.PuntosCategoria
                        };
                        jugadores.Add(jugadorPuntos);
                    }
                }
                catch(Exception e) {
                    return null;
                }
            }
     
            return jugadores;
        }



    }
}
