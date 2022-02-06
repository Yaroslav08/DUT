using AutoMapper;
using DUT.Application.Extensions;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Post;
using DUT.Domain.Models;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class PostService : IPostService
    {
        private readonly DUTDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public PostService(DUTDbContext db, IMapper mapper, IIdentityService identityService)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public async Task<Result<PostViewModel>> CreatePostAsync(PostCreateModel model)
        {
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

        public async Task<Result<List<PostViewModel>>> GetGroupPostsAsync(int groupId, int skip = 0, int count = 20)
        {
            var posts = await _db.Posts
                .AsNoTracking()
                .Where(x => x.GroupId == groupId)
                .Include(x => x.User)
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip).Take(count)
                .ToListAsync();

            return Result<List<PostViewModel>>.SuccessWithData(_mapper.Map<List<PostViewModel>>(posts));
        }

        public async Task<Result<PostViewModel>> GetPostByIdAsync(int postId, int groupId)
        {
            var post = await _db.Posts.AsNoTracking().Include(s => s.User).FirstOrDefaultAsync(x => x.Id == postId);
            if (post == null)
                return Result<PostViewModel>.NotFound("Post not found");

            if (post.GroupId != groupId)
                return Result<PostViewModel>.NotFound("This post not from this group");
            var postToView = _mapper.Map<PostViewModel>(post);
            postToView.CountComments = await _db.PostComments.CountAsync(s => s.PostId == postId);

            return Result<PostViewModel>.SuccessWithData(postToView);
        }

        public async Task<Result<bool>> RemovePostAsync(int postId, int groupId)
        {
            var postToDelete = await _db.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == postId);
            if (postToDelete == null)
                return Result<bool>.NotFound("Post not found");

            if(postToDelete.GroupId != groupId)
                return Result<bool>.NotFound("This group don`t have current post");

            _db.Posts.Remove(postToDelete);
            await _db.SaveChangesAsync();
            return Result<bool>.Success();
        }

        public async Task<Result<PostViewModel>> UpdatePostAsync(PostEditModel model)
        {
            var postToUpdate = await _db.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.Id);
            if (postToUpdate == null)
                return Result<PostViewModel>.NotFound("Post not found");

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
    }
}
