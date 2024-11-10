using Blog.Core.Models;
using Blog.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Runtime.ConstrainedExecution;

namespace Blog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IPostService _postService;
        public PostsController(ILogger<PostsController> logger, IPostService postService)
        {
            _logger = logger;
            _postService = postService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get()
        {
            var posts = await _postService.Get();

            if (!posts.Any())
                return NotFound();

            return Ok(posts);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Post(PostModel postModel)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            await _postService.Create(postModel);

            return CreatedAtAction(nameof(Post), new { id = postModel.Id }, postModel);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put([FromRoute] string id, PostModel postModel)
        {
            if (id != postModel.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                await _postService.Update(id, postModel);

            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex.Message);

                if (!await _postService.PostExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            if (!await _postService.PostExists(id))
                return NotFound();

            await _postService.Delete(id);

            return Ok();
        }

        [HttpGet("{id}/comments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            if (!await _postService.PostExists(id))
                return NotFound();

            return Ok(await _postService.GetPostComments(id));
        }

        [HttpPost("{id}/comments")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PostComments([FromRoute] string id, [FromBody] CommentModel commentModel)
        {
            if (!await _postService.PostExists(id))
                return NotFound();

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (id != commentModel.PostId)
                return BadRequest();

            await _postService.CreatePostComment(id, commentModel);

            return CreatedAtAction(nameof(PostComments), new { id = commentModel.Id }, commentModel);
        }
    }
}