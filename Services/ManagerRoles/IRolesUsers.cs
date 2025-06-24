namespace Preguntin_ASP.NET.Services.ManagerRoles
{
    public interface IRolesUsers
    {
        public Task<string> CrearRolesAsyncDefault();
        public Task<bool> ComprobarRoleExist(ICollection<string> roles);
        public Task<string> ManagerRolAsync(ICollection<string> nombreRole, string option);
        public ICollection<string?> GetRoles();
        public Task<string> UpdateRolAsync(ICollection<string> OldName, ICollection<string> NewName);
    }
}
