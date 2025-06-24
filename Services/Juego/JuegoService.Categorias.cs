using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Preguntin_ASP.NET.Models.DTO;
using Preguntin_ASP.NET.Models.Preguntas;
using Preguntin_ASP.NET.Models.Usuarios;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Preguntin_ASP.NET.Services.Juego
{
    public partial class JuegoService
    {
        //obtenemos nombre y puntos de categoria
        //1. selecciona todas las categorias y las convierte en una lista
        //2. mapea la categoria solo para obtener el nombre y los puntos
        public async Task<ICollection<CategoriaDTO>> ShowCategoria(string idusuario)
        {
            await AsociarJugadorCategoria(idusuario);
            var result = _mapper.Map<List<CategoriaDTO>>(_context.Categoria.ToList());
            foreach(var categoria in result)
            {
                categoria.PuntosCategoria = _context.categoriasUsuarios.Where(p => p.IdCategoria == categoria.IdCategoria&&p.IdUser==idusuario)
                                                                       .Select(p => p.PuntosCategoria)
                                                                       .First();
            }

            return result;
        }

        //asociar el jugador con una categoria 
        private async Task AsociarJugadorCategoria(string idJugador)
        {
            var categorias = _context.Categoria.ToList();

            foreach (var categoria in categorias)
            {
                var comprobarAsociacion = await _context.categoriasUsuarios.AnyAsync(p => p.IdUser == idJugador && p.IdCategoria == categoria.IdCategoria);

                if (!comprobarAsociacion)
                {
                    var categoriaasociada = new CategoriasUsuarios()
                    {
                        IdCategoria = categoria.IdCategoria,
                        IdUser = idJugador,
                        PuntosCategoria = 0
                    };

                    _context.categoriasUsuarios.Add(categoriaasociada);
                }
            }
            await _context.SaveChangesAsync();
        }

        //establecer puntos de la categria 
        public async Task<string> setPuntosCategoriasAsync(string idJugador, int idCategoria, int puntos)
        {
            try
            {
                var jugadorCategoria = _context.categoriasUsuarios.Where(p => p.IdCategoria == idCategoria && p.IdUser == idJugador).First();

                if (jugadorCategoria == null)
                    throw new Exception("El recurso no existe");
                else
                {
                    jugadorCategoria.PuntosCategoria += puntos;
                    await _context.SaveChangesAsync();

                }

                return "Puntos guardados exitosamente";

            }catch(Exception e)
            {
                return e.Message;
            }

        }

        //Da el valor de los puntos al jugador
        public async Task<string> SetPuntosJugadorAsync(string id, int puntos){
            try
            {
                var jugador = _context.jugadorUsers.Where(p => p.Id == id).First();
                jugador.Puntos = puntos;
                await _usuariosManager.UpdateAsync(jugador);
                return "Puntos guardados exitosamente";
            }
            catch (Exception e)
            {
                return e.Message;
            }
           
        } 

        //1. crea las categrias 
        //2. si no existe las guarda por defecto
        public async Task CreateCategoriaDefault()
        {
            List<CategoriaDefaultDTO> categorias = new List<CategoriaDefaultDTO>()
            {
                new CategoriaDefaultDTO {Nombre="General Knowledge"},
                new CategoriaDefaultDTO {Nombre="Entertainment: Books"},
                new CategoriaDefaultDTO {Nombre="Entertainment: Film"},
                new CategoriaDefaultDTO {Nombre="Entertainment: Music"},
                new CategoriaDefaultDTO {Nombre="Entertainment: Musicals: Theatres"},
                new CategoriaDefaultDTO {Nombre="Entertainment: Television"},
                new CategoriaDefaultDTO {Nombre="Entertainment: Video Games"},
                new CategoriaDefaultDTO {Nombre="Entertainment: Board Games"},
                new CategoriaDefaultDTO {Nombre="Science: Nature"},
                new CategoriaDefaultDTO {Nombre="Science: Computers"},
                new CategoriaDefaultDTO {Nombre="Science: Mathematics"},
                new CategoriaDefaultDTO {Nombre="Mythology"},
                new CategoriaDefaultDTO {Nombre="Sports"},
                new CategoriaDefaultDTO {Nombre="Geography"},
                new CategoriaDefaultDTO {Nombre="History"},
                new CategoriaDefaultDTO {Nombre="Politics"},
                new CategoriaDefaultDTO {Nombre="Art"},
            };

            List<ModelCategoria> result = _mapper.Map<List<ModelCategoria>>(categorias);//mapeamos los objectos

            //combrovamos la existencia de las categorias, si no estan se crear por defecto
            foreach(var categoria in result)
            {
                //si existe la categoria devulve true
                if (!await CheckExistCategoriaAsync(categoria.Nombre))
                    _context.Categoria.Add(categoria);
                else
                    Console.WriteLine("Las categoria ya existe");
            }

            _context.SaveChanges();
        }

    

    }
}
