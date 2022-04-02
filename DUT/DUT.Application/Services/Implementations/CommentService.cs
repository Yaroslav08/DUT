using AutoMapper;
using DUT.Application.Extensions;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Post.Comment;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
namespace DUT.Application.Services.Implementations
{
    public class CommentService : BaseService<PostComment>, ICommentService
    {
        private readonly DUTDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public CommentService(DUTDbContext db, IMapper mapper, IIdentityService identityService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<CommentViewModel>> CreateCommentAsync(CommentCreateModel model)
        {
            var newComment = new PostComment
            {
                IsPublic = model.IsPublic,
                PostId = model.PostId,
                UserId = _identityService.GetUserId(),
                Text = model.Text
            };
            newComment.PrepareToCreate(_identityService);
            await _db.PostComments.AddAsync(newComment);
            await _db.SaveChangesAsync();
            return Result<CommentViewModel>.SuccessWithData(_mapper.Map<CommentViewModel>(newComment));
        }

        public async Task<Result<List<CommentViewModel>>> GetCommentsByPostIdAsync(int groupId, int postId, int skip = 0, int count = 20)
        {
            if (!await _db.Posts.AnyAsync(s => s.Id == postId && s.GroupId == groupId))
                return Result<List<CommentViewModel>>.NotFound("Post from this group not found");

            var comments = await _db.PostComments
                .AsNoTracking()
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

            var commentToRemove = await _db.PostComments.FirstOrDefaultAsync(s => s.Id == commentId);
            if (commentToRemove == null)
                return Result<bool>.NotFound("Comment not found");

            if (commentToRemove.PostId != postId)
                return Result<bool>.Error("Access denited");

            if (!_identityService.IsAdministrator())
                if (commentToRemove.UserId != _identityService.GetUserId())
                    return Result<bool>.Error("Access denited");

            _db.PostComments.Remove(commentToRemove);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<CommentViewModel>> UpdateCommentAsync(CommentEditModel model)
        {
            var commentToUpdate = await _db.PostComments.FindAsync(model.Id);
            if (commentToUpdate == null)
                return Result<CommentViewModel>.NotFound("Comment not found");

            if (!_identityService.IsAdministrator())
                if (commentToUpdate.UserId != _identityService.GetUserId())
                    return Result<CommentViewModel>.Error("Access denited");

            commentToUpdate.Text = model.Text;
            commentToUpdate.IsPublic = model.IsPublic;
            commentToUpdate.PrepareToUpdate(_identityService);
            _db.PostComments.Update(commentToUpdate);
            await _db.SaveChangesAsync();
            return Result<CommentViewModel>.SuccessWithData(_mapper.Map<CommentViewModel>(commentToUpdate));
        }
    }
}