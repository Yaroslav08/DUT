using DUT.Domain.Models;
namespace DUT.Application.Services.Interfaces
{
    public interface IRoleService : IBaseService<Role>
    {
        Task<Role> GetRoleByIdAsync(int id);
        Task<Role> GetRoleByNameAsync(string name);
        Task<Role> CreateRoleAsync(Role role);
        Task<Role> UpdateRoleAsync(Role role);
        Task<List<Role>> GetAllRolesAsync();
        Task DeleteRoleAsync(int id);
    }
}