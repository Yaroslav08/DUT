using AutoMapper;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Post;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace URLS.Application.Services.Implementations
{
    public class PostService : BaseService<Post>, IPostService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public PostService(URLSDbContext db, IMapper mapper, IIdentityService identityService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<PostViewModel>> CreatePostAsync(PostCreateModel model)
        {
            var member = await GetMemberAsync(_identityService.GetUserId(), model.GroupId);
            if (member == null)
                return Result<PostViewModel>.Forbiden();
            if (!member.UserGroupRole.Permissions.CanCreatePost)
                return Result<PostViewModel>.Forbiden();

            var newPost = new Post
            {
                Title = model.Title,
                Content = model.Content,
                AvailableToComment = model.AvailableToComment,
                IsImportant = model.IsImportant,
                IsPublic = model.IsPublic,
                GroupId = model.GroupId,
                UserId = _identityService.GetUserId()
            };
            newPost.PrepareToCreate(_identityService);
            await _db.Posts.AddAsync(newPost);
            await _db.SaveChangesAsync();
            return Result<PostViewModel>.SuccessWithData(_mapper.Map<PostViewModel>(newPost));
        }

        public async Task<Result<List<PostViewModel>>> GetPostsByGroupIdAsync(int groupId, int skip = 0, int count = 20)
        {
            var query = _db.Posts.AsNoTracking();

            var member = await GetMemberAsync(_identityService.GetUserId(), groupId);
            if (member == null)
                query = query.Where(x => x.IsPublic);

            query = query.Where(x => x.GroupId == groupId)
                .Include(x => x.User)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip).Take(count);

            var posts = await query.ToListAsync();

            return Result<List<PostViewModel>>.SuccessWithData(_mapper.Map<List<PostViewModel>>(posts));
        }

        public async Task<Result<PostViewModel>> GetPostByIdAsync(int postId, int groupId)
        {
            var post = await _db.Posts.AsNoTracking().Include(s => s.User).FirstOrDefaultAsync(x => x.Id == postId);
            if (post == null)
                return Result<PostViewModel>.NotFound("Post not found");

            if (post.GroupId != groupId)
                return Result<PostViewModel>.NotFound("This post not from this group");

            var member = await GetMemberAsync(_identityService.GetUserId(), groupId);
            if (member == null && !post.IsPublic)
                return Result<PostViewModel>.Forbiden();

            var postToView = _mapper.Map<PostViewModel>(post);
            postToView.CountComments = await _db.Comments.CountAsync(s => s.PostId == postId);

            return Result<PostViewModel>.SuccessWithData(postToView);
        }

        public async Task<Result<bool>> RemovePostAsync(int postId, int groupId)
        {
            var member = await GetMemberAsync(_identityService.GetUserId(), groupId);
            if (member == null)
                return Result<bool>.Forbiden();
            if (!member.UserGroupRole.Permissions.CanRemovePost)
                return Result<bool>.Forbiden();

            var postToDelete = await _db.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == postId);
            if (postToDelete == null)
                return Result<bool>.NotFound("Post not found");

            if (!_identityService.IsAdministrator())
                if (postToDelete.UserId != _identityService.GetUserId())
                    return Result<bool>.Error("Access denited");

            if (postToDelete.GroupId != groupId)
                return Result<bool>.NotFound("This group don`t have current post");

            _db.Posts.Remove(postToDelete);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<PostViewModel>> UpdatePostAsync(PostEditModel model)
        {
            var member = await GetMemberAsync(_identityService.GetUserId(), model.GroupId);
            if (member == null)
                return Result<PostViewModel>.Forbiden();
            if (!member.UserGroupRole.Permissions.CanEditPost)
                return Result<PostViewModel>.Forbiden();

            var postToUpdate = await _db.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.Id);
            if (postToUpdate == null)
                return Result<PostViewModel>.NotFound("Post not found");

            if (!_identityService.IsAdministrator())
                if (postToUpdate.UserId != _identityService.GetUserId() && !member.UserGroupRole.Permissions.CanEditAllPosts)
                    return Result<PostViewModel>.Forbiden();

            postToUpdate.Title = model.Title;
            postToUpdate.Content = model.Content;
            postToUpdate.AvailableToComment = model.AvailableToComment;
            postToUpdate.IsImportant = model.IsImportant;
            postToUpdate.IsPublic = model.IsPublic;
            postToUpdate.PrepareToUpdate(_identityService);

            _db.Posts.Update(postToUpdate);
            await _db.SaveChangesAsync();

            return Result<PostViewModel>.SuccessWithData(_mapper.Map<PostViewModel>(postToUpdate));
        }

        private async Task<UserGroup> GetMemberAsync(int userId, int groupId)
        {
            return await _db.UserGroups.AsNoTracking()
                .Include(s => s.UserGroupRole)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.GroupId == groupId);
        }
    }
}