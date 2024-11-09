using Blog.Core.Models;

namespace Blog.Core.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentModel> GetById(Guid id);
        Task<CommentModel> Update(Guid commentId, CommentModel commentModel);
        Task<Guid?> Delete(Guid id);
        Task<bool> CommentExists(Guid commentId);
    }
}