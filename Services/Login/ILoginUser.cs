using Preguntin_ASP.NET.Models.DTO;

namespace Preguntin_ASP.NET.Services.Login
{
    public interface ILoginUser
    {
        public Task<string> ComprobacionLoginAsync(Models.DTO.Login modelLogin);
    }
}
