using Blog.Core.Models;

namespace Blog.Core.Services.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<PostModel>> Get();
        Task<PostModel> GetById(string id);
        Task<IEnumerable<CommentModel>> GetPostComments(string id);
        Task<PostModel> Create(PostModel post);
        Task<PostModel> Update(string id, PostModel postModel);
        Task<string> Delete(string id);
        Task<CommentModel> CreatePostComment(string postId, CommentModel comment);
        Task<bool> PostExists(string postId);
    }
}