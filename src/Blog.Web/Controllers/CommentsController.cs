using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog.Data.Context;
using Blog.Data.Models;
using Blog.Web.Models;
using System.Security.Claims;

namespace Blog.Web.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("[controller]/editar/{id:int}")]
        public async Task<IActionResult> Edit(int? id)
        {
            var comment = await _context.Comments
                                           .Include(p => p.Author)
                                           .Include(p => p.Post)
                                           .FirstOrDefaultAsync(p => p.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            return View(new CommentModel
            {
                Id = comment.Id,
                AuthorId = comment.AuthorId,
                AuthorName = comment.Author.UserName,
                Content = comment.Content,
                CreateDate = comment.CreateDate,
                PostId = comment.Post.Id
            });
        }

        [HttpPost("[controller]/editar/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content")] CommentModel commentModel)
        {
            if (id != commentModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var comment = await _context.Comments.FindAsync(commentModel.Id);

                    var authorId = User?.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (comment.AuthorId != authorId && !User.IsInRole("Admin"))
                    {
                        return Forbid();
                    }

                    comment.Content = commentModel.Content;
                    comment.CreateDate = DateTime.Now;

                    _context.Update(comment);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(commentModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(actionName: nameof(Index), controllerName: "Posts");
            }
            return View(commentModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            return View(new CommentModel
            {
                Id = comment.Id,
                PostId = comment.PostId,
                AuthorId = comment.AuthorId,
                Content = comment.Content
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var authorId = User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (comment.AuthorId != authorId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(actionName: nameof(Index), controllerName: "Posts");
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
