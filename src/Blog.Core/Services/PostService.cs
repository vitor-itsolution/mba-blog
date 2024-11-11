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
                AuthorId = p.Author?.Id,
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

            var comments = await _context.Comments.Include(p => p.Author)
                                                  .Where(p => p.PostId == id).ToListAsync();

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

        public async Task<PostModel> Create(PostModel postModel)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(p => p.Email == _httpContextAccessor.HttpContext.User.Identity.Name);

            var post = new Post
            {
                Title = postModel.Title,
                Content = postModel.Content,
                CreateDate = postModel.CreateDate,
                AuthorId = author.Id
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            postModel.Id = post.Id;
            postModel.AuthorId = author.Id;
            postModel.AuthorName = author.Name;

            return postModel;
        }

        public async Task<PostModel> Update(string id, PostModel postModel)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(p => p.Email == _httpContextAccessor.HttpContext.User.Identity.Name);

            var post = await _context.Posts.FindAsync(postModel.Id);

            if (post.AuthorId != author.Id && !_httpContextAccessor.HttpContext.User.IsInRole("Admin") || id != postModel.Id)
            {
                throw new UnauthorizedAccessException();
            }

            post.Title = postModel.Title;
            post.Content = postModel.Content;
            post.CreateDate = DateTime.Now;
            post.AuthorId = author.Id;

            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return postModel;
        }

        public async Task<string> Delete(string id)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(p => p.Email == _httpContextAccessor.HttpContext.User.Identity.Name);

            if (!await PostExists(id))
                return null;

            var post = await _context.Posts
                                     .Include(p=> p.Comments)
                                     .FirstOrDefaultAsync(p=> p.Id == id);

            if (post.AuthorId != author.Id && !_httpContextAccessor.HttpContext.User.IsInRole("Admin"))
            {
                throw new UnauthorizedAccessException();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return id;
        }

        public async Task<CommentModel> CreatePostComment(string postId, CommentModel commentModel)
        {
            if (!await PostExists(postId))
                return null;

            if (postId != commentModel.PostId)
                throw new UnauthorizedAccessException();

            var author = await _context.Authors.FirstOrDefaultAsync(p => p.Email == _httpContextAccessor.HttpContext.User.Identity.Name);

            var comment = new Comment
            {
                Content = commentModel.Content,
                PostId = postId,
                CreateDate = DateTime.Now,
                AuthorId = author.Id
            };

            _context.Comments.Add(comment);

            await _context.SaveChangesAsync();

            commentModel.Id = comment.Id;
            commentModel.AuthorName = author.Name;
            commentModel.AuthorId = author.Id;

            return commentModel;
        }

        public async Task<bool> PostExists(string postId)
            => await _context.Posts.AnyAsync(p => p.Id == postId);
    }
}