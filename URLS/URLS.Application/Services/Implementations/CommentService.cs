using AutoMapper;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Post.Comment;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using URLS.Constants.Extensions;

namespace URLS.Application.Services.Implementations
{
    public class CommentService : BaseService<Comment>, ICommentService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly IPermissionCommentService _permissionCommentService;
        private readonly IGroupService _groupService;
        private readonly IPostService _postService;
        public CommentService(URLSDbContext db, IMapper mapper, IIdentityService identityService, IPermissionCommentService permissionCommentService, IGroupService groupService, IPostService postService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
            _permissionCommentService = permissionCommentService;
            _groupService = groupService;
            _postService = postService;
        }

        public async Task<Result<CommentViewModel>> CreateCommentAsync(CommentCreateModel model)
        {
            if (!await _groupService.IsExistAsync(s => s.Id == model.GroupId))
                return Result<CommentViewModel>.NotFound(typeof(Group).NotFoundMessage(model.GroupId));

            if (!await _postService.IsExistAsync(s => s.Id == model.PostId))
                return Result<CommentViewModel>.NotFound(typeof(Post).NotFoundMessage(model.PostId));

            if (!await _permissionCommentService.CanCreateCommentAsync(model.GroupId))
                return Result<CommentViewModel>.Forbiden();

            var newComment = new Comment
            {
                IsPublic = model.IsPublic,
                PostId = model.PostId,
                UserId = _identityService.GetUserId(),
                Text = model.Text
            };
            newComment.PrepareToCreate(_identityService);
            await _db.Comments.AddAsync(newComment);
            await _db.SaveChangesAsync();
            return Result<CommentViewModel>.SuccessWithData(_mapper.Map<CommentViewModel>(newComment));
        }

        public async Task<Result<List<CommentViewModel>>> GetCommentsByPostIdAsync(int groupId, int postId, int skip = 0, int count = 20)
        {
            if (!await _db.Posts.AnyAsync(s => s.Id == postId && s.GroupId == groupId))
                return Result<List<CommentViewModel>>.NotFound("Post from this group not found");

            var query = _db.Comments.AsNoTracking();

            if (!await _permissionCommentService.CanViewAllCommentsAsync(groupId, postId))
                query = query.Where(s => s.IsPublic);

            var comments = await query
                .Where(x => x.PostId == postId)
                .OrderByDescending(s => s.CreatedAt)
                .Include(x => x.User)
                .Skip(skip).Take(count)
                .ToListAsync();

            return Result<List<CommentViewModel>>.SuccessWithData(_mapper.Map<List<CommentViewModel>>(comments));
        }

        public async Task<Result<bool>> RemoveCommentAsync(int groupId, int postId, long commentId)
        {
            if (!await _db.Posts.AsNoTracking().AnyAsync(s => s.Id == postId && s.GroupId == groupId))
                return Result<bool>.NotFound("Post from this group not found");

            var commentToRemove = await _db.Comments.FirstOrDefaultAsync(s => s.Id == commentId);
            if (commentToRemove == null)
                return Result<bool>.NotFound("Comment not found");

            if (commentToRemove.PostId != postId)
                return Result<bool>.Error("Access denited");

            if (!_identityService.IsAdministrator())
                if (commentToRemove.UserId != _identityService.GetUserId())
                    return Result<bool>.Error("Access denited");

            _db.Comments.Remove(commentToRemove);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<CommentViewModel>> UpdateCommentAsync(CommentEditModel model)
        {
            var commentToUpdate = await _db.Comments.FindAsync(model.Id);
            if (commentToUpdate == null)
                return Result<CommentViewModel>.NotFound("Comment not found");

            if (!_identityService.IsAdministrator())
                if (commentToUpdate.UserId != _identityService.GetUserId())
                    return Result<CommentViewModel>.Error("Access denited");

            commentToUpdate.Text = model.Text;
            commentToUpdate.IsPublic = model.IsPublic;
            commentToUpdate.PrepareToUpdate(_identityService);
            _db.Comments.Update(commentToUpdate);
            await _db.SaveChangesAsync();
            return Result<CommentViewModel>.SuccessWithData(_mapper.Map<CommentViewModel>(commentToUpdate));
        }
    }
}