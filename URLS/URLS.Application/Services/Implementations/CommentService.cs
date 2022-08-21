using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Post.Comment;
using URLS.Constants.APIResponse;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly URLSDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly IPermissionCommentService _permissionCommentService;
        private readonly ICommonService _commonService;
        public CommentService(URLSDbContext db, IMapper mapper, IIdentityService identityService, IPermissionCommentService permissionCommentService, ICommonService commonService)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
            _permissionCommentService = permissionCommentService;
            _commonService = commonService;
        }

        public async Task<Result<CommentViewModel>> CreateCommentAsync(CommentCreateModel model)
        {
            if (!await _commonService.IsExistAsync<Group>(s => s.Id == model.GroupId))
                return Result<CommentViewModel>.NotFound(typeof(Group).NotFoundMessage(model.GroupId));

            if (!await _commonService.IsExistAsync<Post>(s => s.Id == model.PostId))
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
            return Result<CommentViewModel>.Created(_mapper.Map<CommentViewModel>(newComment));
        }

        public async Task<Result<List<CommentViewModel>>> GetCommentsByPostIdAsync(int groupId, int postId, int skip = 0, int count = 20)
        {
            if (!await _db.Posts.AnyAsync(s => s.Id == postId && s.GroupId == groupId))
                return Result<List<CommentViewModel>>.NotFound(typeof(Post).NotFoundMessage(postId));

            var query = _db.Comments.AsNoTracking();

            if (!await _permissionCommentService.CanViewAllCommentsAsync(groupId, postId))
                query = query.Where(s => s.IsPublic);

            var comments = await query
                .Where(x => x.PostId == postId)
                .OrderByDescending(s => s.CreatedAt)
                .Include(x => x.User)
                .Skip(skip).Take(count)
                .ToListAsync();

            var totalCount = await _db.Comments.CountAsync(x => x.PostId == postId);

            return Result<List<CommentViewModel>>.SuccessList(_mapper.Map<List<CommentViewModel>>(comments), Meta.FromMeta(totalCount, skip, count));
        }

        public async Task<Result<bool>> RemoveCommentAsync(int groupId, int postId, long commentId)
        {
            if (!await _db.Posts.AsNoTracking().AnyAsync(s => s.Id == postId && s.GroupId == groupId))
                return Result<bool>.NotFound("Post from this group not found");

            var commentToRemove = await _db.Comments.FirstOrDefaultAsync(s => s.Id == commentId);
            if (commentToRemove == null)
                return Result<bool>.NotFound(typeof(Comment).NotFoundMessage(commentId));

            if (commentToRemove.PostId != postId)
                return Result<bool>.Forbiden();

            if (!_identityService.IsAdministrator())
                if (commentToRemove.UserId != _identityService.GetUserId())
                    return Result<bool>.Forbiden();

            _db.Comments.Remove(commentToRemove);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<CommentViewModel>> UpdateCommentAsync(CommentEditModel model)
        {
            var commentToUpdate = await _db.Comments.FindAsync(model.Id);
            if (commentToUpdate == null)
                return Result<CommentViewModel>.NotFound(typeof(Comment).NotFoundMessage(model.Id));

            if (!_identityService.IsAdministrator())
                if (commentToUpdate.UserId != _identityService.GetUserId())
                    return Result<CommentViewModel>.Forbiden();

            commentToUpdate.Text = model.Text;
            commentToUpdate.IsPublic = model.IsPublic;
            commentToUpdate.PrepareToUpdate(_identityService);
            _db.Comments.Update(commentToUpdate);
            await _db.SaveChangesAsync();
            return Result<CommentViewModel>.SuccessWithData(_mapper.Map<CommentViewModel>(commentToUpdate));
        }
    }
}