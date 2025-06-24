using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Preguntin_ASP.NET.Services.Admin;


namespace Preguntin_ASP.NET.Controllers.Admin
{

    [ApiController]
    [Route("Admin/[Controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AdminService admin;

        public UsersController(AdminService adminService) {
            admin = adminService;
        }

        [Authorize(Policy ="Admin")]
        [HttpGet("AllUsers")]
        public async Task<IActionResult> GetAllUsersAsync()  {

           try { 
                return Ok(await admin.GetAllUsers()); 
           }
           catch (Exception e) { 
                return BadRequest(e.Message); 
           }
        }
 
        [Authorize(Policy = "Admin")]
        [HttpPut("UpdateRoleUser/{id}")]
        public async Task<IActionResult> UpdateRoleUserAsync(string id, [FromBody] string[] roles)
        {
            try{
                await admin.UpdateRole(id, roles);
                string mensaje = "todo salio bien";
                return Ok(new { mensaje});
            }
            catch (Exception ex){
                return BadRequest(new {ex.Message});
            }
        }

        [Authorize(Policy = "Admin")]
        [HttpDelete("DeleteUser/{Id}")]
        public async Task<IActionResult> DelectUserAsync(string Id)
        {
            try{
                await admin.DeleteUserIdAsync(Id);
                return Ok("el usuario se elimino correctamente");

            }catch(Exception e){
                return BadRequest(e.Message);
            }
        }
    }
}
