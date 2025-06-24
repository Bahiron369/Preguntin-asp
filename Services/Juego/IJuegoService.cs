using Preguntin_ASP.NET.Models.DTO;

namespace Preguntin_ASP.NET.Services.Juego
{
    public interface IJuegoService
    {
        public Task<List<PuntosJugador>> GetPointGlobalAsync();
        public Task CreateCategoriaDefault();
        //public Task<string> SavePreguntaAsync(ResponseHttp preguntas);
        public Task<bool> CheckExistQuestionAsync(string nombrePregunta);
        public Task<bool> CheckExistResposeAsync(string texto);
        public Task<bool> CheckExistCategoriaAsync(string nombre);
        public Task<ICollection<PreguntasResponse>> GetQuestionAllAsync(string nombreCategoria);
        public Task<ICollection<CategoriaDTO>> ShowCategoria(string idJugador);
        public Task<string> SetPuntosJugadorAsync(string id, int puntos);
        public Task<string> setPuntosCategoriasAsync(string idJugador, int idCategoria, int puntos);
        public Task<List<PuntosJugador>> GetPointGlobalCategoryAsync(int idCategory);
        public Task SaveQuestionDbAsync(ICollection<PreguntasResponse> preguntasHttp, string nombreCategoria);
        public ICollection<PreguntasResponse> GetQuestionsDb(int idCategoria, string nombreCategoria);


    }
}
