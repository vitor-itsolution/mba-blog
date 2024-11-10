using System.Security.Claims;
using Blog.Core.Models;
using Blog.Core.Services.Interfaces;
using Blog.Data.Context;
using Blog.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Blog.Core.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PostService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<PostModel>> Get()
        {
            var posts = await _context.Posts
                          .Include(p => p.Author)
                          .Include(p => p.Comments)
                          .OrderByDescending(p => p.CreateDate)
                          .AsNoTracking().ToListAsync();

            return posts?.Select(p => new PostModel
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CreateDate = p.CreateDate,
                AmountComment = p.Comments.Count(),
                AuthorId = p.Author.Id,
                AuthorName = p.Author?.Name!
            });
        }

        public async Task<PostModel> GetById(string id)
        {
            if (!await PostExists(id))
                return null;

            var post = await _context.Posts
                              .Include(p => p.Author)
                              .FirstOrDefaultAsync(p => p.Id == id.ToString());

            return new PostModel
            {
                Id = post.Id,
                Title = post.Title,
                AuthorId = post.AuthorId,
                AuthorName = post.Author.Name,
                Content = post.Content,
                CreateDate = post.CreateDate
            };
        }

        public async Task<IEnumerable<CommentModel>> GetPostComments(string id)
        {

            if (!await PostExists(id))
                return null;

            var comments = await _context.Comments.Where(p => p.PostId == id).ToListAsync();

            return comments?.Select(p => new CommentModel
            {
                Id = p.Id,
                PostId = p.PostId,
                Content = p.Content,
                AuthorId = p.AuthorId,
                AuthorName = p.Author?.Name!,
                CreateDate = p.CreateDate
            });
        }

        public async Task<PostModel> Create(PostModel post)
        {
            var authorId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            _context.Posts.Add(new Post
            {
                Title = post.Title,
                Content = post.Content,
                CreateDate = post.CreateDate,
                AuthorId = authorId
            });

            await _context.SaveChangesAsync();

            return post;
        }

        public async Task<PostModel> Update(string id, PostModel postModel)
        {
            var authorId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var post = await _context.Posts.FindAsync(postModel.Id);

            if (post.AuthorId != authorId && !_httpContextAccessor.HttpContext.User.IsInRole("Admin") || id != postModel.Id)
            {
                throw new UnauthorizedAccessException();
            }

            post.Title = postModel.Title;
            post.Content = postModel.Content;
            post.CreateDate = DateTime.Now;
            post.AuthorId = authorId;
            
            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return postModel;
        }

        public async Task<string> Delete(string id)
        {
            var authorId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!await PostExists(id))
                return null;

            var post = await _context.Posts.FindAsync(id);

            if (post.AuthorId != authorId && !_httpContextAccessor.HttpContext.User.IsInRole("Admin"))
            {
                throw new UnauthorizedAccessException();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return id;
        }

        public async Task<CommentModel> CreatePostComment(string postId, CommentModel comment)
        {
            if (!await PostExists(postId))
                return null;

            var authorId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            _context.Comments.Add(new Comment
            {
                Content = comment.Content,
                PostId = postId,
                CreateDate = DateTime.Now,
                AuthorId = authorId
            });

            await _context.SaveChangesAsync();

            return comment;
        }

        public async Task<bool> PostExists(string postId)
            => await _context.Posts.AnyAsync(p => p.Id == postId);
    }
}