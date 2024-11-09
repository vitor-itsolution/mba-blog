using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Core.Models;
using Blog.Core.Services.Interfaces;
using System.Text.Json;

namespace Blog.Web.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        [Route("[controller]/editar/{id}")]
        public async Task<IActionResult> Edit(string id)
        {

            if (!await _commentService.CommentExists(id))
            {
                return NotFound();
            }

            return View(await _commentService.GetById(id));
        }

        [HttpPost("[controller]/editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Content")] CommentModel commentModel)
        {
            if (id != commentModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _commentService.Update(id, commentModel);
                }
                catch (UnauthorizedAccessException)
                {
                    return Forbid();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _commentService.CommentExists(id))
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

        public async Task<IActionResult> Delete(string id)
        {
            if (!await _commentService.CommentExists(id))
            {
                return NotFound();
            }
            return View(await _commentService.GetById(id));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                if (!await _commentService.CommentExists(id))
                {
                    return NotFound();
                }

                await _commentService.Delete(id);

                return RedirectToAction(actionName: nameof(Index), controllerName: "Posts");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(JsonSerializer.Serialize(ex));
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(JsonSerializer.Serialize(ex));
                return View("Ocorreu um erro ao processar a solicitação, tente novamente mais tarde");
            }

        }
    }
}
