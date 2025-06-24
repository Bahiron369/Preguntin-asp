using Preguntin_ASP.NET.Models.DTO;

namespace Preguntin_ASP.NET.Services.HttpCliente
{
    public interface IApiHttpService
    {
        public Task<ResponseHttp> GetPreguntaAsync(string nombreCategoria);
    }
}
