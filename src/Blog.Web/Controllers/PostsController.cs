using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data.Context;
using Blog.Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Blog.Core.Models;
using Blog.Core.Services.Interfaces;

namespace Blog.Web.Controllers
{
    [Authorize]

    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPostService _postService;

        public PostsController(ApplicationDbContext context, IPostService postService)
        {
            _context = context;
            _postService = postService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            ViewData["AuthorId"] = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["IsAdmin"] = User?.IsInRole("Admin");

            return View(await _postService.Get());
        }

        [AllowAnonymous]
        [Route("[controller]/{id:Guid}/comments")]
        public async Task<IActionResult> Comments(Guid id)
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

        [Route("[controller]/{id:Guid}/novo-comentario")]
        public async Task<IActionResult> CreateComment(Guid id)
        {
            if (!await _postService.PostExists(id))
            {
                return NotFound();
            }

            ViewData["PostId"] = id;
            return View();
        }

        [HttpPost("[controller]/{id:Guid}/novo-comentario")]
        public async Task<IActionResult> CreateComment([Bind("Content")] CommentModel commentModel, Guid id)
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
        public async Task<IActionResult> Create([Bind("Id,Title,Content")] PostModel postModel)
        {
            if (ModelState.IsValid)
            {
                await _postService.Create(postModel);

                return RedirectToAction(nameof(Index));
            }

            return View(postModel);
        }

        [Route("[controller]/editar/{id:Guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (!await _postService.PostExists(id))
            {
                return NotFound();
            }

            return View(await _postService.GetById(id));
        }

        [HttpPost("[controller]/editar/{id:Guid}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Content,CreateDate")] PostModel postModel)
        {
            if (id != postModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _postService.Update(postModel);
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

        [Route("[controller]/excluir/{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!await _postService.PostExists(id))
            {
                return NotFound();
            }

            var post = await _context.Posts.FirstOrDefaultAsync(m => m.Id == id);

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

        [HttpPost("[controller]/excluir/{id:Guid}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (!await _postService.PostExists(id))
            {
                return NotFound();
            }

            await _postService.Delete(id);

            return RedirectToAction(actionName: nameof(Index), controllerName: "Posts");
        }
    }
}
