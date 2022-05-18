using Microsoft.EntityFrameworkCore;
using URLS.Application.Services.Interfaces;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class PermissionCommentService : IPermissionCommentService
    {
        private readonly URLSDbContext _db;
        private readonly IIdentityService _identityService;
        public PermissionCommentService(URLSDbContext db, IIdentityService identityService)
        {
            _db = db;
            _identityService = identityService;
        }

        public async Task<bool> CanCreateCommentAsync(int groupId)
        {
            if (_identityService.IsAdministrator())
                return true;

            var member = await _db.UserGroups
                .AsNoTracking()
                .Include(s => s.UserGroupRole)
                .FirstOrDefaultAsync(s => s.Status == Domain.Models.UserGroupStatus.Member && s.GroupId == groupId && s.UserId == _identityService.GetUserId());

            if (member == null)
            {
                var subjects = await _db.Subjects.AsNoTracking().Where(s => s.TeacherId == _identityService.GetUserId() && s.GroupId == groupId).ToListAsync();
                return subjects == null || subjects.Count == 0 ? false : true;
            }
            else
            {
                return member.UserGroupRole.Permissions.CanCreateComment;
            }
        }

        public async Task<bool> CanViewAllCommentsAsync(int groupId, int postId)
        {
            if (_identityService.IsAdministrator())
                return true;

            var member = await _db.UserGroups
                .AsNoTracking()
                .Include(s => s.UserGroupRole)
                .FirstOrDefaultAsync(s => s.Status == Domain.Models.UserGroupStatus.Member && s.GroupId == groupId && s.UserId == _identityService.GetUserId());

            if (member == null)
            {
                var subjects = await _db.Subjects.AsNoTracking().Where(s => s.TeacherId == _identityService.GetUserId() && s.GroupId == groupId).ToListAsync();
                return subjects == null || subjects.Count == 0 ? false : true;
            }
            else
            {
                return member.UserGroupRole.Permissions.CanCreateComment;
            }
        }
    }
}