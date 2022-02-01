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
            var roles = _identityService.GetRoles();
            if (roles.Contains(Roles.Admin))
                return true;

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
