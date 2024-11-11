using Blog.Core.Models;

namespace Blog.Core.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentModel> GetById(string id);
        Task<CommentModel> Update(string commentId, CommentModel commentModel);
        Task<string> Delete(string id);
        Task<bool> CommentExists(string commentId);
    }
}