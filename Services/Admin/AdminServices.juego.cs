using Microsoft.Identity.Client;
using Preguntin_ASP.NET.Models.DTO;
using Preguntin_ASP.NET.Models.Preguntas;
using System.Reflection.Metadata.Ecma335;

namespace Preguntin_ASP.NET.Services.Admin
{
    public partial class AdminService
    {
        /// <summary>
        /// Metodo para guardar pregunta junto con una nueva categoria (esta ultima es opcional)
        /// </summary>
        /// <param name="preguntas"></param>
        /// <param name="nombreCategoria"></param>
        /// <returns></returns>
        public async Task<string> setPreguntas(List<PreguntasResponse> preguntas, string nombreCategoria)
        {
            
            var checkCategoria = await _juego.CheckExistCategoriaAsync(nombreCategoria);

            if (checkCategoria == null || checkCategoria == false)
            {
                var newCategory = new ModelCategoria
                {
                    Nombre = nombreCategoria
                };

                _context.Categoria.Add(newCategory);
                await _context.SaveChangesAsync();
            }

            try
            {
                await _juego.SaveQuestionDbAsync(preguntas, nombreCategoria);
                return "preguntas guardadas exitosamente";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        public async Task<string> DeleteCategoria(int idCategoria)
        {
            try
            {
                var categoria = _context.Categoria.Where(p => p.IdCategoria == idCategoria).First();
                _context.Categoria.Remove(categoria);
                await _context.SaveChangesAsync();
                return "la categoria se elimino correctamente";
            }
            catch(Exception e)
            {
                return e.Message;
            }
           
        }

        //elimina la pregunta por el ID
        public async Task<string> DeleteQuestion(int idPregunta)
        {
            try
            {
                var pregunta = _context.Preguntas.Where(p => p.IdPregunta == idPregunta).First();
                _context.Preguntas.Remove(pregunta);
                await _context.SaveChangesAsync();
                return "la pregunta se elimino correctamente";
            }
            catch (Exception e)
            {
                return e.Message;
            }
           
        }


        /// <summary>
        /// Elimina todas preguntas y respuesta
        /// </summary>
        public async Task<string> deleteQuestionAllAsync(int idcategoria)
        {
            try
            {
                //obtener todas las preguntas y sus respuestas
                var todasLasPreguntas = _context.Preguntas.Where(p => p.IdCategoria == idcategoria).ToList();
                var todasLasRespuestas = _context.Respuestas.Where(p => p.IdRespuesta == todasLasPreguntas[0].IdPregunta).ToList();

                //eliminar todas las preguntas y respuestas
                _context.Respuestas.RemoveRange(todasLasRespuestas);
                _context.Preguntas.RemoveRange(todasLasPreguntas);
                //guardar cambios
                await _context.SaveChangesAsync();
                return "Los datos se borraron correctamente";

            }catch(Exception e)
            {
                return "Error al eliminar datos";
            }
        }

    }
}
