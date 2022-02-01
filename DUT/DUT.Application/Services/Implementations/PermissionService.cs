using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Identity;
using DUT.Constants;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class PermissionService : IPermissionService
    {
        private readonly DUTDbContext _db;
        private readonly IIdentityService _identityService;
        public PermissionService(DUTDbContext db, IIdentityService identityService)
        {
            _db = db;
            _identityService = identityService;
        }

        public async Task<bool> HasPermissionAsync(PermissionAction action, object data = null)
        {
            var role = _identityService.GetRole();

            if (action == PermissionAction.CreateUniversity)
            {
                if (role == Roles.Admin)
                    return true;
            }
            if (action == PermissionAction.EditUniversity)
            {
                if (role == Roles.Admin)
                    return true;
            }
            if (action == PermissionAction.CreateFaculty)
            {
                if (role == Roles.Admin)
                    return true;
            }
            if (action == PermissionAction.EditFaculty)
            {
                if (role == Roles.Admin)
                    return true;
            }
            if (action == PermissionAction.RemoveFaculty)
            {
                if (role == Roles.Admin)
                    return true;
            }
            if (action == PermissionAction.CreateSpecialty)
            {
                if (role == Roles.Admin)
                    return true;
            }
            if (action == PermissionAction.EditSpecialty)
            {
                if (role == Roles.Admin)
                    return true;
            }
            if (action == PermissionAction.RemoveSpecialty)
            {
                if (role == Roles.Admin)
                    return true;
            }
            if (action == PermissionAction.CreateGroup)
            {
                if (role == Roles.Teacher || role == Roles.Moderator || role == Roles.Admin)
                    return true;
            }
            if (action == PermissionAction.EditGroup)
            {
                if (role == Roles.Moderator || role == Roles.Admin)
                    return true;
                else
                {
                    int groupId = Convert.ToInt32(data);
                    int currentUserId = _identityService.GetUserId();
                    var userGroup = await _db.UserGroups
                        .AsNoTracking()
                        .Include(x => x.UserGroupRole)
                        .FirstOrDefaultAsync(x => x.UserId == currentUserId && x.GroupId == groupId);
                    if (userGroup == null)
                        return false;

                    var groupRole = userGroup.UserGroupRole;

                    if (groupRole.Permissions.CanEditInfo)
                    {
                        return true;
                    }
                }
            }
            if (action == PermissionAction.RemoveGroup)
            {
                if (role == Roles.Admin)
                    return true;
            }
            return false;
        }

        public async Task<bool> HasPermissionAsync(string claimType, string claimValue, object data = null)
        {
            UserIdentity currentUser = _identityService.GetUserDetails();
            if (currentUser.IsAdministrator)
                return true;



            return currentUser.Claims.Any(x => x.Type == claimType && x.Value == claimValue);
        }
    }
}
