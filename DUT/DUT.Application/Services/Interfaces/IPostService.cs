using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Post;
namespace DUT.Application.Services.Interfaces
{
    public interface IPostService
    {
        Task<Result<List<PostViewModel>>> GetGroupPostsAsync(int groupId, int skip = 0, int count = 20);
        Task<Result<PostViewModel>> GetPostByIdAsync(int postId, int groupId);
        Task<Result<PostViewModel>> CreatePostAsync(PostCreateModel model);
        Task<Result<PostViewModel>> UpdatePostAsync(PostEditModel model);
        Task<Result<bool>> RemovePostAsync(int postId);
    }
}