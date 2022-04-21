using Microsoft.EntityFrameworkCore;
using URLS.Application.Services.Interfaces;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class PermissionPostService : IPermissionPostService
    {
        private readonly URLSDbContext _db;
        private readonly IIdentityService _identityService;
        public PermissionPostService(URLSDbContext db, IIdentityService identityService)
        {
            _db = db;
            _identityService = identityService;
        }

        public async Task<bool> CanCreatePostAsync(int groupId)
        {
            if (_identityService.IsAdministrator())
                return true;
            var member = await _db.UserGroups
                .AsNoTracking()
                .Include(s => s.UserGroupRole)
                .FirstOrDefaultAsync(s => s.GroupId == groupId && s.UserId == _identityService.GetUserId() && s.Status == UserGroupStatus.Member);

            if (member == null) // check for teacher
            {
                var subjects = await _db.Subjects.Where(s => !s.IsTemplate && s.TeacherId == _identityService.GetUserId() && s.GroupId == groupId).Select(s => new Subject { Id = s.Id, Name = s.Name }).ToListAsync();
                if (subjects == null || subjects.Count == 0)
                    return false;
            }
            else // for member
            {
                if (!member.UserGroupRole.Permissions.CanCreatePost)
                    return false;
            }
            return true;
        }

        public async Task<bool> CanRemovePostAsync(int groupId, Post post)
        {
            if (_identityService.IsAdministrator())
                return true;

            var member = await _db.UserGroups
                .AsNoTracking()
                .Include(s => s.UserGroupRole)
                .FirstOrDefaultAsync(s => s.GroupId == groupId && s.UserId == _identityService.GetUserId() && s.Status == UserGroupStatus.Member);

            if (member != null) // for member
            {
                if (post.UserId != _identityService.GetUserId() && !member.UserGroupRole.Permissions.CanRemoveAllPosts)
                    return false;
            }
            else // for teacher
            {
                if (post.UserId != _identityService.GetUserId())
                    return false;
            }
            return true;
        }

        public async Task<bool> CanRemovePostAsync(int groupId, int postId)
        {
            var postToRemove = await _db.Posts.AsNoTracking().FirstOrDefaultAsync(s => s.Id == postId);
            if (postToRemove == null)
                return false;
            return await CanRemovePostAsync(groupId, postToRemove);
        }

        public async Task<bool> CanUpdatePostAsync(int groupId, Post post)
        {
            if (_identityService.IsAdministrator())
                return true;

            var member = await _db.UserGroups
                .AsNoTracking()
                .Include(s => s.UserGroupRole)
                .FirstOrDefaultAsync(s => s.GroupId == groupId && s.UserId == _identityService.GetUserId() && s.Status == UserGroupStatus.Member);

            if (member != null) // for member
            {
                if (post.UserId != _identityService.GetUserId() && !member.UserGroupRole.Permissions.CanUpdateAllPosts)
                    return false;
            }
            else // for teacher
            {
                if (post.UserId != _identityService.GetUserId())
                    return false;
            }
            return true;
        }

        public async Task<bool> CanUpdatePostAsync(int groupId, int postId)
        {
            var postToUpdate = await _db.Posts.AsNoTracking().FirstOrDefaultAsync(s => s.Id == postId);
            if (postToUpdate == null)
                return false;
            return await CanUpdatePostAsync(groupId, postToUpdate);
        }

        public async Task<bool> CanViewOnlyPublicPostsAsync(int groupId)
        {
            if (_identityService.IsAdministrator())
                return false;
            var isMember = await _db.UserGroups
                .AsNoTracking()
                .AnyAsync(s => s.GroupId == groupId && s.UserId == _identityService.GetUserId() && s.Status == UserGroupStatus.Member);
            var subjects = await _db.Subjects.AsNoTracking().Where(s => s.GroupId == groupId && s.TeacherId == _identityService.GetUserId()).ToListAsync();

            if (isMember || (subjects != null && subjects.Count > 0) || _identityService.IsAdministrator())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CanViewPost(Post post)
        {
            if (_identityService.IsAdministrator())
                return true;

            if (post.UserId == _identityService.GetUserId())
                return true;

            return false;
        }

        public async Task<bool> CanViewPostAsync(int postId)
        {
            var postToView = await _db.Posts.AsNoTracking().FirstOrDefaultAsync(s => s.Id == postId);
            if (postToView == null)
                return false;
            return CanViewPost(postToView);
        }
    }
}