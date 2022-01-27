using DUT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DUT.Application.Services.Interfaces
{
    public interface IRoleService
    {
        Task<Role> GetRoleByIdAsync(int id);
        Task<Role> GetRoleByNameAsync(string name);
        Task<Role> CreateRoleAsync(Role role);
        Task<Role> UpdateRoleAsync(Role role);
        Task<List<Role>> GetAllRolesAsync();
        Task DeleteRoleAsync(int id);
    }
}