using Microsoft.AspNetCore.Identity;

namespace Preguntin_ASP.NET.Services.ManagerRoles
{
    public class RolesUsers : IRolesUsers
    {

        private readonly RoleManager<IdentityRole> _roles;
        public RolesUsers(RoleManager<IdentityRole> roles) {
            _roles = roles;    
        }

        public async Task<string> CrearRolesAsyncDefault()
        {
            List<string> roles = ["Admin", "Jugador"];

            foreach (var rol in roles) {
                if(await _roles.RoleExistsAsync(rol)) //verificamos si existe un rol               
                    throw new Exception("El rol ya existe");                
                else
                    await _roles.CreateAsync(new IdentityRole(rol));    //creamos un rol
            }
            return "el rol fue creado con exito";
        }

       //agregar o eliminar roles
        public async Task<string> ManagerRolAsync(ICollection<string> nombreRole, string option)
        {
            if (nombreRole == null)
                throw new Exception("Valor nulo");

            switch (option)
            {
                case "CreateRole":
                    if (await ComprobarRoleExist(nombreRole)) throw new Exception("El rol ya existe");
                    break;

                case "DeleteRole":
                    if (!await ComprobarRoleExist(nombreRole)) throw new Exception("El rol no existe");
                    break;
            };

            foreach (var rol in nombreRole)
            {
                if(option=="CreateRole")
                    await _roles.CreateAsync(new IdentityRole(rol));
                if (option == "DeleteRole")
                {
                    IdentityRole? rolName = await _roles.FindByNameAsync(rol);
                    await _roles.DeleteAsync(rolName);

                }

            }

            return "Operacion exitosa";
        }

        //actualizar roles
        public async Task<string> UpdateRolAsync(ICollection<string> OldName, ICollection<string> NewName)
        {

            if (OldName == null || NewName==null || !await ComprobarRoleExist(OldName))
                throw new Exception("El rol no existe");

            for(int i = 0; i<OldName.Count;i++)
            {
                IdentityRole? rol = await _roles.FindByNameAsync(OldName.ElementAt(i));//obtenemos rol por el nombre
                rol.Name = NewName.ElementAt(i); //agregamos el nuevo nombre
                await _roles.UpdateAsync(rol); //actualizamos
            }

            return "Operacion exitosa";
        }

        public ICollection<string?> GetRoles() =>  _roles.Roles.Select(r => r.Name).ToList(); //obtenemos los nombres
        
        //comprueba la existencia de roles
        public async Task<bool> ComprobarRoleExist(ICollection<string> role)
        {
            foreach(var rol in role)
            {
                if(!await _roles.RoleExistsAsync(rol)) 
                    return false;
            }
            return true;

        }
    }
}
