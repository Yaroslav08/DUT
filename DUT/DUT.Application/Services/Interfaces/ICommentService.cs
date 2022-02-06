using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Post.Comment;

namespace DUT.Application.Services.Interfaces
{
    public interface ICommentService
    {
        Task<Result<List<CommentViewModel>>> GetPostCommentsAsync(int postId, int skip = 0, int count = 20);
        Task<Result<CommentViewModel>> CreateCommentAsync(CommentCreateModel model);
        Task<Result<CommentViewModel>> UpdateCommentAsync(CommentEditModel model);
        Task<Result<bool>> RemoveCommentAsync(long commentId);
    }
}