using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Post;
using DUT.Domain.Models;
namespace DUT.Application.Services.Interfaces
{
    public interface IPostService : IBaseService<Post>
    {
        Task<Result<List<PostViewModel>>> GetPostsByGroupIdAsync(int groupId, int skip = 0, int count = 20);
        Task<Result<PostViewModel>> GetPostByIdAsync(int postId, int groupId);
        Task<Result<PostViewModel>> CreatePostAsync(PostCreateModel model);
        Task<Result<PostViewModel>> UpdatePostAsync(PostEditModel model);
        Task<Result<bool>> RemovePostAsync(int postId, int groupId);
    }
}