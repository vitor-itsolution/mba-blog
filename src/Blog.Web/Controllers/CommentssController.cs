using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data.Context;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Blog.Web.Models;

namespace Blog.Web.Controllers
{
    [Authorize]
    [Controller]
    public class CommentssController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _authorId;

        public CommentssController(ApplicationDbContext context)
        {
            _context = context;
            _authorId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // [Route("editar/{id:int}")]
        // public async Task<IActionResult> Edit(int id)
        // {
        //     var comment = await _context.Comments
        //                                    .Include(p => p.Author)
        //                                    .Include(p => p.Post)
        //                                    .FirstOrDefaultAsync(p => p.Id == id);

        //     if (comment == null)
        //     {
        //         return NotFound();
        //     }

        //     return View(new CommentModel
        //     {
        //         Id = comment.Id,
        //         AuthorId = comment.AuthorId,
        //         AuthorName = comment.Author.UserName,
        //         Content = comment.Content,
        //         CreateDate = comment.CreateDate,
        //         PostId = comment.Post.Id
        //     });
        // }

        // [HttpPost("editar/{id:int}")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> EditComment(int id, [Bind("Id,PostId,AuthorId,Content,CreateDate")] CommentModel commentModel)
        // {
        //     if (id != commentModel.Id)
        //     {
        //         return NotFound();
        //     }

        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             var comment = await _context.Comments.FindAsync(commentModel.Id);

        //             var authorId = User?.FindFirstValue(ClaimTypes.NameIdentifier);

        //             if (comment.AuthorId != authorId && !User.IsInRole("Admin"))
        //             {
        //                 return Forbid();
        //             }

        //             comment.Content = commentModel.Content;
        //             comment.CreateDate = DateTime.Now;

        //             _context.Update(comment);
        //             await _context.SaveChangesAsync();

        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!PostExists(commentModel.Id))
        //             {
        //                 return NotFound();
        //             }
        //             else
        //             {
        //                 throw;
        //             }
        //         }
        //         return RedirectToAction(actionName: nameof(Index), controllerName: "Posts");
        //     }
        //     return View(commentModel);
        // }

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

        private bool PostExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
