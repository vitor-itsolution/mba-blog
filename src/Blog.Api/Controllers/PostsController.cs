using Blog.Api.Models;
using Blog.Core.Models;
using Blog.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Controllers
{
    [Authorize]
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

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get()
        {
            return Ok(await _postService.Get());
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Post(CreatePostViewModel createPost)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var postModel = new PostModel { Title = createPost.Title, Content = createPost.Content };

            await _postService.Create(postModel);

            return CreatedAtAction(nameof(Post), new { id = postModel.Id }, postModel);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put([FromRoute] string id, UpdatePostViewModel updatePost)
        {
            if (id != updatePost.Id)
                return BadRequest();

            if (!await _postService.PostExists(id))
                return NotFound();

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                await _postService.Update(id, new PostModel
                {
                    Id = updatePost.Id,
                    Title = updatePost.Title,
                    Content = updatePost.Content
                });

            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex.Message);

                if (!await _postService.PostExists(id))
                    return NotFound();
                else
                    throw;
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            if (!await _postService.PostExists(id))
                return NotFound();

            try
            {
                await _postService.Delete(id);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            return NoContent();
        }

        [AllowAnonymous]
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PostComments([FromRoute] string id, [FromBody] CreatePostCommentViewModel createPostComment)
        {
            if (!await _postService.PostExists(id))
                return NotFound();

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (id != createPostComment.PostId)
                return BadRequest();

            var commentModel = new CommentModel
            {
                PostId = createPostComment.PostId,
                Content = createPostComment.Content
            };
            
            commentModel = await _postService.CreatePostComment(id, commentModel);

            return CreatedAtAction(nameof(PostComments), new { id = commentModel.Id }, commentModel);
        }
    }
}