using AutoMapper;
using DUT.Application.Services.Interfaces;
using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Post.Comment;
using DUT.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DUT.Application.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly DUTDbContext _db;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        public CommentService(DUTDbContext db, IMapper mapper, IIdentityService identityService)
        {
            _db = db;
            _mapper = mapper;
            _identityService = identityService;
        }

        public Task<Result<CommentViewModel>> CreateCommentAsync(CommentCreateModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<List<CommentViewModel>>> GetPostCommentsAsync(int postId, int skip = 0, int count = 20)
        {
            var comments = await _db.PostComments
                .AsNoTracking()
                .Where(x => x.PostId == postId)
                .Include(x => x.User)
                .Skip(skip).Take(count)
                .ToListAsync();

            return Result<List<CommentViewModel>>.SuccessWithData(_mapper.Map<List<CommentViewModel>>(comments));
        }

        public Task<Result<bool>> RemoveCommentAsync(long postId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CommentViewModel>> UpdateCommentAsync(CommentEditModel model)
        {
            throw new NotImplementedException();
        }
    }
}
