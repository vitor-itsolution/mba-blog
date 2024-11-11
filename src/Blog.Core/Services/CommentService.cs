using Blog.Core.Models;
using Blog.Core.Services.Interfaces;
using Blog.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Blog.Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CommentService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommentModel> GetById(string id)
        {
            var comment = await _context.Comments
                               .Include(p => p.Author)
                               .Include(p => p.Post)
                               .FirstOrDefaultAsync(p => p.Id == id.ToString());

            return new CommentModel
            {
                Id = comment.Id,
                AuthorId = comment.AuthorId,
                AuthorName = comment.Author.Name,
                Content = comment.Content,
                CreateDate = comment.CreateDate,
                PostId = comment.Post.Id
            };
        }

        public async Task<CommentModel> Update(string commentId, CommentModel commentModel)
        {
            if (!await CommentExists(commentModel.Id))
                return null;


            var author = await _context.Authors.FirstOrDefaultAsync(p => p.Email == _httpContextAccessor.HttpContext.User.Identity.Name);
            var comment = await _context.Comments.FindAsync(commentModel.Id);

            if (comment.AuthorId != author.Id && !_httpContextAccessor.HttpContext.User.IsInRole("Admin") || commentId != commentModel.Id)
            {
                throw new UnauthorizedAccessException();
            }

            comment.Content = commentModel.Content;

            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();

            return commentModel;

        }

        public async Task<string> Delete(string id)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(p => p.Email == _httpContextAccessor.HttpContext.User.Identity.Name);

            if (!await CommentExists(id))
                return null;

            var comment = await _context.Comments.FindAsync(id);

            if (comment.AuthorId != author.Id && !_httpContextAccessor.HttpContext.User.IsInRole("Admin"))
            {
                throw new UnauthorizedAccessException();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return id;
        }

        public async Task<bool> CommentExists(string commentId)
           => await _context.Comments.AnyAsync(p => p.Id == commentId);
    }
}