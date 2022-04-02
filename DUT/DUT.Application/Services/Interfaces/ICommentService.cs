﻿using DUT.Application.ViewModels;
using DUT.Application.ViewModels.Post.Comment;
using DUT.Domain.Models;

namespace DUT.Application.Services.Interfaces
{
    public interface ICommentService : IBaseService<Comment>
    {
        Task<Result<List<CommentViewModel>>> GetCommentsByPostIdAsync(int groupId, int postId, int skip = 0, int count = 20);
        Task<Result<CommentViewModel>> CreateCommentAsync(CommentCreateModel model);
        Task<Result<CommentViewModel>> UpdateCommentAsync(CommentEditModel model);
        Task<Result<bool>> RemoveCommentAsync(int groupId, int postId, long commentId);
    }
}