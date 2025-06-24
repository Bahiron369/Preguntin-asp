using Microsoft.AspNetCore.Mvc;
using Preguntin_ASP.NET.Services.Login;

namespace Preguntin_ASP.NET.Controllers.Login
{
    [ApiController]
    [Route("Account/[Controller]")]
    public class LoginController : Controller
    {

        public readonly ILoginUser login;

        public LoginController(ILoginUser login){

            this.login = login;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Models.DTO.Login loginModel)
        {
            try{
                string token = await login.ComprobacionLoginAsync(loginModel);
                return Ok(new { token });
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
