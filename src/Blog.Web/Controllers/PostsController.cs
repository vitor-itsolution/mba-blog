using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data.Context;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Blog.Core.Models;
using Blog.Core.Services.Interfaces;
using System.Text.Json;

namespace Blog.Web.Controllers
{
    [Authorize]

    public class PostsController : Controller
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostsController> _Logger;

        public PostsController(ApplicationDbContext context, IPostService postService, ILogger<PostsController> logger)
        {
            _postService = postService;
            _Logger = logger;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            ViewData["AuthorId"] = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["IsAdmin"] = User?.IsInRole("Admin");

            return View(await _postService.Get());
        }

        [AllowAnonymous]
        [Route("[controller]/{id}/comments")]
        public async Task<IActionResult> Comments(string id)
        {
            if (!await _postService.PostExists(id))
            {
                return NotFound();
            }

            ViewData["AuthorId"] = User?.FindFirstValue(ClaimTypes.NameIdentifier); ;
            ViewData["IsAdmin"] = User?.IsInRole("Admin");
            ViewData["PostId"] = id;

            return View(await _postService.GetPostComments(id));
        }

        [Route("[controller]/{id}/novo-comentario")]
        public async Task<IActionResult> CreateComment(string id)
        {
            if (!await _postService.PostExists(id))
            {
                return NotFound();
            }

            ViewData["PostId"] = id;
            return View(new CommentModel { PostId = id });
        }

        [HttpPost("[controller]/{id}/novo-comentario")]
        public async Task<IActionResult> CreateComment([Bind("Content, PostId")] CommentModel commentModel, string id)
        {
            if (ModelState.IsValid)
            {
                if (!await _postService.PostExists(id))
                {
                    return NotFound();
                }

                await _postService.CreatePostComment(id, commentModel);

                return RedirectToAction(nameof(Index));
            }

            return View(commentModel);
        }

        [Route("[controller]/novo")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("[controller]/novo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content")] PostModel postModel)
        {
            if (ModelState.IsValid)
            {
                await _postService.Create(postModel);

                return RedirectToAction(nameof(Index));
            }

            return View(postModel);
        }

        [Route("[controller]/editar/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (!await _postService.PostExists(id))
            {
                return NotFound();
            }

            return View(await _postService.GetById(id));
        }

        [HttpPost("[controller]/editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Content,CreateDate")] PostModel postModel)
        {
            if (id != postModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _postService.Update(id, postModel);
                }
                catch (UnauthorizedAccessException)
                {
                    return Forbid();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _postService.PostExists(postModel.Id))
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
            return View(postModel);
        }

        [Route("[controller]/excluir/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!await _postService.PostExists(id))
            {
                return NotFound();
            }

            var post = await _postService.GetById(id);

            return View(new PostModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreateDate = post.CreateDate,
                AuthorId = post.AuthorId,
                // AuthorName = post.Author?.UserName
            });
        }

        [HttpPost("[controller]/excluir/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                if (!await _postService.PostExists(id))
                {
                    return NotFound();
                }

                await _postService.Delete(id);

                return RedirectToAction(actionName: nameof(Index), controllerName: "Posts");
            }
            catch (UnauthorizedAccessException ex)
            {
                _Logger.LogError(JsonSerializer.Serialize(ex));
                return View("Error", $"Permissão negada");
            }
            catch (Exception ex)
            {
                _Logger.LogError(JsonSerializer.Serialize(ex));
                return View("Ocorreu um erro ao processar a solicitação, tente novamente mais tarde");
            }

        }
    }
}
