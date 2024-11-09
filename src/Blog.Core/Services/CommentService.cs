using System.Security.Claims;
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

        public async Task<CommentModel> GetById(Guid id)
        {
            var comment = await _context.Comments
                               .Include(p => p.Author)
                               .Include(p => p.Post)
                               .FirstOrDefaultAsync(p => p.Id == id);

            return new CommentModel
            {
                Id = comment.Id,
                AuthorId = comment.AuthorId,
                AuthorName = comment.Author.UserName,
                Content = comment.Content,
                CreateDate = comment.CreateDate,
                PostId = comment.Post.Id
            };
        }

        public async Task<CommentModel> Update(Guid commentId, CommentModel commentModel)
        {
            if (!await CommentExists(commentModel.Id))
                return null;


            var authorId = _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var comment = await _context.Comments.FindAsync(commentModel.Id);

            if (comment.AuthorId != authorId && !_httpContextAccessor.HttpContext.User.IsInRole("Admin") || commentId != commentModel.Id)
            {
                throw new UnauthorizedAccessException();
            }

            comment.Content = commentModel.Content;
            comment.CreateDate = DateTime.Now;

            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();

            return commentModel;

        }

        public async Task<Guid?> Delete(Guid id)
        {
            var authorId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await  CommentExists(id))
                return null;

            var comment = await _context.Comments.FindAsync(id);

            if (comment.AuthorId != authorId && !_httpContextAccessor.HttpContext.User.IsInRole("Admin"))
            {
                throw new UnauthorizedAccessException();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return id;
        }

        public async Task<bool> CommentExists(Guid commentId)
           => await _context.Comments.AnyAsync(p => p.Id == commentId);
    }
}