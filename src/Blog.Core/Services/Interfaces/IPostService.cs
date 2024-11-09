using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Core.Models;

namespace Blog.Core.Services.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<PostModel>> Get();
        Task<PostModel> GetById(Guid id);
        Task<IEnumerable<CommentModel>> GetPostComments(Guid id);
        Task<PostModel> Create(PostModel post);
        Task<PostModel> Update(PostModel postModel);
        Task<Guid?> Delete(Guid id);
        Task<CommentModel> CreatePostComment(Guid postId, CommentModel comment);
        Task<bool> PostExists(Guid postId);
    }
}