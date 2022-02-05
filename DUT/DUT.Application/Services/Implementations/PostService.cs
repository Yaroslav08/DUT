using AutoMapper;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Post;
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
            throw new NotImplementedException();
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

        public async Task<Result<bool>> RemovePostAsync(int postId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<PostViewModel>> UpdatePostAsync(PostEditModel model)
        {
            throw new NotImplementedException();
        }
    }
}
