using Blog.Core.Models;
using Blog.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;

        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Register(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            if (await _authService.AuthorExists(registerUser.Email)) return Problem("E-mail já cadastrado");

            var token = await _authService.Register(registerUser);

            if (string.IsNullOrWhiteSpace(token))
                return Problem("Falha ao registrar o usuário");

            return Created(nameof(Register), token);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Login(LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var token = await _authService.Login(loginUser);

            if (string.IsNullOrWhiteSpace(token))
                return Problem("Usuário ou senha incorretos");

            return Ok(token);
        }
    }
}