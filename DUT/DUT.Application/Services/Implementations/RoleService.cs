using DUT.Application.Services.Interfaces;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly DUTDbContext _db;
        public RoleService(DUTDbContext db)
        {
            _db = db;
        }

        public async Task<Role> CreateRoleAsync(Role role)
        {
            await _db.Roles.AddAsync(role);
            await _db.SaveChangesAsync();
            return role;
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _db.Roles.FindAsync(id);
            if (role == null)
                return;
            _db.Roles.Remove(role);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _db.Roles.AsNoTracking().ToListAsync();
        }

        public async Task<Role> GetRoleByIdAsync(int id)
        {
            return await _db.Roles.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Role> GetRoleByNameAsync(string name)
        {
            return await _db.Roles.AsNoTracking().FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task<Role> UpdateRoleAsync(Role role)
        {
            var roleToUpdate = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(s => s.Id == role.Id);
            if(roleToUpdate == null)
                return null;
            roleToUpdate.Name = role.Name;
            _db.Roles.Update(roleToUpdate);
            await _db.SaveChangesAsync();
            return roleToUpdate;
        }
    }
}
