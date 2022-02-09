using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels.Identity;
using DUT.Constants;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;

namespace DUT.Application.Services.Implementations
{
    public class PermissionService : BaseService<User>, IPermissionService
    {
        private readonly DUTDbContext _db;
        private readonly IIdentityService _identityService;
        public PermissionService(DUTDbContext db, IIdentityService identityService) : base(db)
        {
            _db = db;
            _identityService = identityService;
        }

        public async Task<bool> HasPermissionAsync(string claimType, string claimValue, object data = null)
        {
            UserIdentity currentUser = _identityService.GetUserDetails();
            
            return HasPermission(claimType, claimValue, currentUser);
        }

        public bool HasPermission(string claimType, string claimValue, UserIdentity currentUser = null)
        {
            currentUser = currentUser ?? _identityService.GetUserDetails();
            if (currentUser.IsAdministrator)
                return true;
            if (currentUser.Claims.Any(s => s.Type == claimType && s.Value == Permissions.All))
                return true;
            return currentUser.Claims.Any(x => x.Type == claimType && x.Value == claimValue);
        }
    }
}
