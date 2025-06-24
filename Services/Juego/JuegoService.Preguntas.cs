using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Identity.Client;
using Preguntin_ASP.NET.Models.DTO;
using Preguntin_ASP.NET.Models.Preguntas;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Preguntin_ASP.NET.Services.Juego
{
    public partial class JuegoService
    {

        //**********************************************************************************************************//
        // estos metodos guardan la respuestas obtenida por la api public (donde obtenemos las preguntas) y lo guarda en la base de datos
        //1. Guarda la preguntas y sube los cambios
        //2. Guarda las respuestas asociadas con la pregunta
        public async Task SaveQuestionDbAsync(ICollection<PreguntasResponse> preguntasHttp,string nombreCategoria)
        {
            ICollection<ModelPregunta?> modelPreguntas = _mapper.Map<List<ModelPregunta?>>(preguntasHttp);    

            if (modelPreguntas == null)
                throw new Exception("No se encontro la categoria");

            //guardamos los datos en el modelo 
            foreach (var pregunta in modelPreguntas)
            {
                //si la pregunta existe que no la guarde 
                if (await CheckExistQuestionAsync(pregunta.Nombre))
                    continue;

                pregunta.IdCategoria = _context.Categoria.Where(p=>p.Nombre==nombreCategoria).First().IdCategoria;

                //obtenemos la pregunta correcta
                string? textoRespuestaCorrecta = preguntasHttp.Select(p => p.respuestasCorrecta).First();

                //Guardamos preguntas antes que guardar la respuesta, eso nos ayuda a evitar conflictos
                await _context.Preguntas.AddAsync(pregunta); //agregamos el modelo a la entidad
                await _context.SaveChangesAsync();

                //Obtener preguntas incorrectas
                ICollection<string?> textoRespuestasIncorrectas = preguntasHttp.Where(p=>p.Nombre==pregunta.Nombre).First().respuestasIncorrecta;
                await SaveResponseInQuestionAsync(textoRespuestaCorrecta, pregunta.IdPregunta, true);

                foreach (var respuestasIncorrectas in textoRespuestasIncorrectas)
                    await SaveResponseInQuestionAsync(respuestasIncorrectas, pregunta.IdPregunta, false);

            }
            await _context.SaveChangesAsync(); //guardamos todos los cambios de la pregunta y respuesta
        }

        //Guarda la respuesta
        private async Task SaveResponseInQuestionAsync(string textoRespuesta, int idPregunta, bool RespuestaCorrecta)
        {
            //agregamos respuesta correcta
            var respuestaSave = new ModelRespuesta
            {
                IdPregunta = idPregunta,
                texto = textoRespuesta,
                isRespuestaCorrecta = RespuestaCorrecta
            };
            await _context.Respuestas.AddAsync(respuestaSave);
        }

        //**********************************************************************************************************//
        //Estos metodos nos permite enviar al Frontend las preguntas y respuestas de dos fuentes
        //1. De la API Publica 2.De la base de datos (En caso de que la primera no funcione)
        //El tercer metodo es el que decide cual usar 
        //El 4 metodo controla los errores en la solicitud de la API

        // estos metodos nos permite enviar al frontend las preguntas y respuesta que fueron obtemidas de la API
        private async Task<ICollection<PreguntasResponse>> GetQuestionHttpAsync(ResponseHttp preguntasHttp)
        {
            LanzarErrorSolicitudHttp(preguntasHttp);

            ICollection<PreguntasResponse> preguntas = new List<PreguntasResponse>();

            foreach(var preguntaHttp in preguntasHttp.results)
            {
                //agregamos propiedades de las respuestas con los valores
                PreguntasResponse preguntaResponse = new PreguntasResponse
                {
                    Tipo = preguntaHttp.type,
                    dificultad = preguntaHttp.difficulty,
                    NombreCategoria = preguntaHttp.category,
                    Nombre = preguntaHttp.question,
                    TiempoRespuesta = new TimeSpan(0, 1, 30),
                    PuntoPregunta = preguntaHttp.difficulty switch
                    {
                        "easy" => 150,
                        "medium" => 200,
                        "hard" => 300
                    },
                    respuestasCorrecta = preguntaHttp.correct_answer,
                    respuestasIncorrecta = preguntaHttp.incorrect_answers
                };
 
                preguntas.Add(preguntaResponse);
            }
            return preguntas;
        }

        // estos metodos nos permite enviar al frontend las preguntas y respuesta que fueron obtemidas de la base de datos
        private ICollection<PreguntasResponse> GetQuestionsDb(string nombreCategoria)
        {
            //obtenemos la informacion de la base de datos
            int Idcategoria = _context.Categoria.Where(p => p.Nombre == nombreCategoria).First().IdCategoria; //Seleciona el id de la categoria que sea igual al nombre
            List<ModelPregunta> preguntas = _context.Preguntas.Where(p => p.IdCategoria == Idcategoria).Take(10).ToList(); //seleciona las primeras 10 preguntas que sean iguales al Id
            ICollection<PreguntasResponse> preguntasHttp = new List<PreguntasResponse>();

            foreach (var pregunta in preguntas)
            {
                //agregamos propiedades de las respuestas con los valores
                PreguntasResponse preguntaResponse = new PreguntasResponse
                {
                    Tipo = pregunta.Tipo,
                    dificultad = pregunta.dificultad,
                    NombreCategoria = nombreCategoria,
                    Nombre = pregunta.Nombre,
                    TiempoRespuesta = pregunta.TiempoRespuesta,
                    PuntoPregunta = pregunta.PuntoPregunta,
                    respuestasCorrecta = _context.Respuestas.Where(p => p.isRespuestaCorrecta == true && p.IdPregunta == pregunta.IdPregunta).First().texto,

                };
                //agregamos la respuesta incorrecta
                preguntaResponse.respuestasIncorrecta = 
                    _context.Respuestas.Where(p => p.isRespuestaCorrecta == false && p.IdPregunta == pregunta.IdPregunta)
                                       .Select(p => p.texto)
                                       .ToList();

                preguntasHttp.Add(preguntaResponse);
            }
            return preguntasHttp;
        }

        // estos metodos nos permite enviar al frontend las preguntas y respuesta que fueron obtemidas de la base de datos
        public ICollection<PreguntasResponse> GetQuestionsDb(int idCategoria,string nombreCategoria)
        {
            List<ModelPregunta> preguntas = _context.Preguntas.Where(p => p.IdCategoria == idCategoria).ToList(); //seleciona las primeras 10 preguntas que sean iguales al Id
            ICollection<PreguntasResponse> preguntasHttp = new List<PreguntasResponse>();

            foreach (var pregunta in preguntas)
            {
                //agregamos propiedades de las respuestas con los valores
                PreguntasResponse preguntaResponse = new PreguntasResponse
                {
                    Id = pregunta.IdPregunta,
                    Tipo = pregunta.Tipo,
                    dificultad = pregunta.dificultad,
                    Nombre = pregunta.Nombre,
                    TiempoRespuesta = pregunta.TiempoRespuesta,
                    PuntoPregunta = pregunta.PuntoPregunta,

                };

                preguntasHttp.Add(preguntaResponse);
            }
            return preguntasHttp;
        }

        //decide en donde obtener la fuente de la base de datos
        public async Task<ICollection<PreguntasResponse>> GetQuestionAllAsync(string nombreCategoria)
        {
            try
            {
                ResponseHttp preguntas = await _apiHttpService.GetPreguntaAsync(nombreCategoria);
                var preguntasResponse = await GetQuestionHttpAsync(preguntas);
                await SaveQuestionDbAsync(preguntasResponse, nombreCategoria);
                return preguntasResponse;

            }catch(Exception e){

                return GetQuestionsDb(nombreCategoria);
            }
        }

        // controlar errores en la solicitud
        private void LanzarErrorSolicitudHttp(ResponseHttp preguntasHttp)
        {
            if (preguntasHttp == null)
                throw new Exception("valor nulo");

            if (preguntasHttp.response_code != 0)
                throw new Exception("Sin resultados");
        }

        /// <summary>
        /// Estos metodos comprueba la existencia de una categoria, pregunta o respuesta
        /// </summary>
        //comprobamos si hay una pregunta 
        public async Task<bool> CheckExistQuestionAsync(string nombrePregunta) => await _context.Preguntas.AnyAsync(p => p.Nombre ==nombrePregunta);

        //comprueba si hay una respuesta
        public async Task<bool> CheckExistResposeAsync(string texto) => await _context.Respuestas.AnyAsync(p => p.texto == texto);
        //comprovar si hay una respuesta
        public async Task<bool> CheckExistCategoriaAsync(string nombre) => await _context.Categoria.AnyAsync(p => p.Nombre == nombre);
        
    }
}
