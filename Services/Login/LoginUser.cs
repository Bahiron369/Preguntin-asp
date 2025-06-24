using Microsoft.AspNetCore.Identity;
using Preguntin_ASP.NET.Models.DTO;
using Preguntin_ASP.NET.Models.Usuarios;
using Preguntin_ASP.NET.Services.Token;

namespace Preguntin_ASP.NET.Services.Login
{
    public class LoginUser : ILoginUser
    {
        public readonly UserManager<Jugador> userManager;
        public readonly ITokenJwt token;
        public readonly SignInManager<Jugador> signInManager;

        public LoginUser(UserManager<Jugador> userManager, ITokenJwt token, SignInManager<Jugador> signInManager)
        {
            this.userManager = userManager;
            this.token = token;
            this.signInManager = signInManager;
        }

        public async Task<string> ComprobacionLoginAsync(Models.DTO.Login modelLogin)
        {
            //comprobamos email
            Jugador? user = await userManager.FindByEmailAsync(modelLogin.Email); 
            if (user == null)
                throw new Exception("Correo electronico no registrado");

            //comprovamos contraseña
            SignInResult validacion = await signInManager.CheckPasswordSignInAsync(user,modelLogin.password,true);
            if (validacion.Succeeded)
                return await token.CreateTokenAsync(user);
            else
                throw new Exception("Contraseña incorrecta");
            
        }
    }
}
