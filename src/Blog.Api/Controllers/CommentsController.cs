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
    public class CommentsController : ControllerBase
    {
        private readonly ILogger<CommentsController> _logger;
        private readonly ICommentService _commentService;

        public CommentsController(ILogger<CommentsController> logger, ICommentService commentService)
        {
            _logger = logger;
            _commentService = commentService;
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put([FromRoute] string id, CommentModel commentModel)
        {
            if (id != commentModel.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                await _commentService.Update(id, commentModel);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex.Message);

                if (!await _commentService.CommentExists(id))
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

            if (!await _commentService.CommentExists(id))
                return NotFound();

            try
            {
                await _commentService.Delete(id);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

            return NoContent();
        }

    }
}