using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Preguntin_ASP.NET.Models.DTO;
using Preguntin_ASP.NET.Services.ManagerRoles;
using System.Data;

namespace Preguntin_ASP.NET.Controllers.Admin
{

    [ApiController]
    [Route("Admin/[Controller]")]
    public class RolesController : Controller
    {
        public readonly IRolesUsers rolesUsers;

        public RolesController(IRolesUsers rolesUsers)
        {
            this.rolesUsers = rolesUsers;
        }

        //mostrar errores
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public IActionResult GetRolesAsync() => Ok(rolesUsers.GetRoles());


        //agregar un nuevo rol
        [Authorize(Policy = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] Roles roles)
        {
            try
            {
                return Ok(await rolesUsers.ManagerRolAsync(roles.Name, "CreateRole"));
            }
            catch (Exception e) { return BadRequest(e.Message); }
        }

        //eliminar un rol existente  
        [Authorize(Policy = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> DeleteRole([FromBody] Roles roles)
        {
            try
            {
                return Ok(await rolesUsers.ManagerRolAsync(roles.Name, "DeleteRole"));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //eliminar un rol existente  
        [Authorize(Policy = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] Roles roles)
        {
            try
            {
                return Ok(await rolesUsers.UpdateRolAsync(roles.Name,roles.NewName));
            }
            catch (Exception e) { return BadRequest(e.Message); }
        }
    }

}
