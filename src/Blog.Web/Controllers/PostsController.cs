using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data.Context;
using Blog.Data.Models;
using Blog.Web.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Blog.Web.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;

        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            ViewData["AuthorId"] = User?.FindFirstValue(ClaimTypes.NameIdentifier); ;

            var posts = await _context.Posts
                                      .Include(p => p.Author)
                                      .Include(p => p.Comments)
                                      .OrderByDescending(p => p.CreateDate).ToListAsync();

            return View(posts.Select(p => new PostModel
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CreateDate = p.CreateDate,
                AmountComment = p.Comments.Count(),
                AuthorId = p.Author?.Id!,
                AuthorName = p.Author?.UserName!
            }));
        }

        [AllowAnonymous]
        [Route("{id:int}/comments")]
        public async Task<IActionResult> Comments(int id)
        {
            var post = await _context.Posts
                                     .Include(p => p.Comments)
                                     .Include(p => p.Author)
                                     .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            ViewData["AuthorId"] = User?.FindFirstValue(ClaimTypes.NameIdentifier); ;
            ViewData["PostId"] = post.Id;

            var comments = post.Comments.Select(p => new CommentModel
            {
                Id = p.Id,
                PostId = p.PostId,
                Content = p.Content,
                AuthorId = p.AuthorId,
                AuthorName = p.Author?.UserName!,
                CreateDate = p.CreateDate
            }).ToList();

            return View(comments);
        }

        [Route("posts/{id:int}/novo-comentario")]
        public async Task<IActionResult> CreateComment(int id)
        {
            var post = await _context.Posts
                                     .Include(p => p.Comments)
                                     .Include(p => p.Author)
                                     .FirstOrDefaultAsync(p => p.Id == id);


            if (post == null)
            {
                return NotFound();
            }

            ViewData["PostId"] = post.Id;
            return View();
        }

        [HttpPost("posts/{id:int}/novo-comentario")]
        public async Task<IActionResult> CreateComment([Bind("Content")] CommentModel commentModel, int id)
        {
            if (ModelState.IsValid)
            {
                var post = await _context.Posts
                                     .Include(p => p.Comments)
                                     .Include(p => p.Author)
                                     .FirstOrDefaultAsync(p => p.Id == id);

                if (post == null)
                {
                    return NotFound();
                }
                var authorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                post.Comments.Add(new Comment
                {
                    Content = commentModel.Content,
                    PostId = id,
                    CreateDate = DateTime.Now,
                    AuthorId = authorId
                });

                _context.Update(post);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(commentModel);
        }

        [Route("novo")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("novo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content")] PostModel postModel)
        {
            if (ModelState.IsValid)
            {
                var authorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var post = new Post
                {
                    Title = postModel.Title,
                    Content = postModel.Content,
                    CreateDate = postModel.CreateDate,
                    AuthorId = authorId
                };

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(postModel);
        }
        [Route("editar/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts.Include(p => p.Author)
                                           .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(new PostModel
            {
                Id = post.Id,
                Title = post.Title,
                AuthorId = post.AuthorId,
                AuthorName = post.Author.UserName,
                Content = post.Content,
                CreateDate = post.CreateDate
            });
        }

        [HttpPost("editar/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CreateDate")] PostModel postModel)
        {
            if (id != postModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var post = await _context.Posts.FindAsync(postModel.Id);

                    var authorId = User?.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (post.AuthorId != authorId && !User.IsInRole("Admin"))
                    {
                        return Forbid();
                    }

                    post.Title = postModel.Title;
                    post.Content = postModel.Content;
                    post.CreateDate = DateTime.Now;
                    post.AuthorId = authorId;

                    _context.Update(post);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(postModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(postModel);
        }
        [Route("excluir/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(new PostModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreateDate = post.CreateDate,
                AuthorId = post.AuthorId,
                AuthorName = post.Author?.UserName
            });
        }

        [HttpPost("excluir/{id:int}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            var authorId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (post.AuthorId != authorId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
