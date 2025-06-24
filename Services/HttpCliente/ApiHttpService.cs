using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Preguntin_ASP.NET.Models.DTO;
using static System.Reflection.Metadata.BlobBuilder;
using System.Text.RegularExpressions;
using System.Net;
using System.Text.Json;
using Preguntin_ASP.NET.Models.Preguntas;
using Microsoft.EntityFrameworkCore;
using Preguntin_ASP.NET.Services.Juego;
using Preguntin_ASP.NET.Data;

namespace Preguntin_ASP.NET.Services.HttpCliente
{
    public class ApiHttpService : IApiHttpService
    {

        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        public ApiHttpService(HttpClient httpClient, ApplicationDbContext context) { 
            _httpClient = httpClient;
            _context = context;
        }

        private  int GetCategoriaList(string nombreCategoria)
        {
            //relacionamos el nombre de la categoria con el Id de la api publica 
            Dictionary<string, int> categorias = new Dictionary<string, int>()
            {
                {"General Knowledge", 9},
                {"Entertainment: Books", 10},
                {"Entertainment: Film", 11},
                {"Entertainment: Music", 12},
                {"Entertainment: Musicals: Theatres", 13},
                {"Entertainment: Television", 14},
                {"Entertainment: Video Games", 15},
                {"Entertainment: Board Games", 16},
                {"Science: Nature", 17},
                {"Science: Computers", 18},
                {"Science: Mathematics", 19},
                {"Mythology", 20},
                {"Sports", 21},
                {"Geography", 22},
                {"History", 23},
                {"Politics", 24},
                {"Art", 25},
                {"Animals", 26},
                {"Vehicles", 27}

            };

            return categorias[nombreCategoria];
        }
        public async Task<ResponseHttp> GetPreguntaAsync(string nombreCategoria)
        {
             HttpResponseMessage? result = new HttpResponseMessage();
             result = await _httpClient.GetAsync($"?amount=10&category={GetCategoriaList(nombreCategoria)}"); //obtenemos contenido de la api
        
            if (result.StatusCode == HttpStatusCode.TooManyRequests) throw new Exception(result.StatusCode.ToString());//si salio mal lanza una excepcion
            string? content = await result.Content.ReadAsStringAsync(); //convertimos de json a string
            ResponseHttp responseHttp = JsonSerializer.Deserialize<ResponseHttp>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive=true}); //damos el valor deserializadp

            //convertimos en simbolos las expresiones regulares 
            foreach(PreguntasHttp question in responseHttp.results)
            {
                question.type = WebUtility.HtmlDecode(question.type);
                question.difficulty = WebUtility.HtmlDecode(question.difficulty);
                question.category = WebUtility.HtmlDecode(question.category);
                question.question = WebUtility.HtmlDecode(question.question);
                question.correct_answer = WebUtility.HtmlDecode(question.correct_answer);

                foreach(string? value in question.incorrect_answers)    
                    question.incorrect_answers.FirstOrDefault(value);
            }
            return responseHttp;
        }

    }
}
