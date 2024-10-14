using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data.Context;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Blog.Web.Models;

namespace Blog.Web.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _authorId;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
            _authorId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
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

            if (comment.AuthorId != _authorId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(actionName: nameof(Index), controllerName: "Posts");
        }
    }
}
