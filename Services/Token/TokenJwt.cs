using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Preguntin_ASP.NET.Models;
using Preguntin_ASP.NET.Models.Usuarios;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Preguntin_ASP.NET.Services.Token
{
    public class TokenJwt : ITokenJwt
    {
        private readonly UserManager<Jugador> _userManager;
        private readonly JwtModel _jwtModel;

        public TokenJwt(UserManager<Jugador> userManager, IOptions<JwtModel> options) {
            _userManager = userManager;
            _jwtModel = options.Value;
        }

        /// <summary>
        /// Los claim son las propiedades del usuario y se crean con la informacion del usuario
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<IEnumerable<Claim>> CreateClaimsTokenAsync(Jugador user)
        {
            List<Claim> Claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id), //registramos id dentro del token
                new Claim(JwtRegisteredClaimNames.Email, user.Email), //registramos el Email
                new Claim(ClaimTypes.Email,value:user.Email),
                new Claim(ClaimTypes.Name, user.UserName), //Registramos el nombre
                new Claim(ClaimTypes.NameIdentifier, user.Id) //registramos el nombre identificador

            };

            IList<string?> roles = await _userManager.GetRolesAsync(user);
            Claims.AddRange(roles.Select(rol => new Claim(ClaimTypes.Role, rol)));

            return Claims;
        }

        /// <summary>
        /// este metodo crea la clave secreta y la encripta en el algoritmo HmacSha256
        /// </summary>
        /// <returns></returns>
        private SigningCredentials CreateCifradoTokenAsync()
        {
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtModel.Secret));//crea la clave
            var credencial = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);//firma la clave con algorimo de cifrado 
            return credencial;
        }

        /// <summary>
        /// Se crea el token con las propiedades del usuario, roles, clave secreta, audiencia, publico y tiempo de expiracion, se envia en formato de cadena
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> CreateTokenAsync(Jugador user)
        {
            IEnumerable<Claim> reglamaciones = await CreateClaimsTokenAsync(user);
            var cred = CreateCifradoTokenAsync();

            //creamos el token
            JwtSecurityToken token = new JwtSecurityToken(

                issuer: _jwtModel.Issuer,
                audience: _jwtModel.Audience,
                claims: reglamaciones,
                expires: DateTime.UtcNow.AddMinutes(_jwtModel.ExpirationMinute),
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
