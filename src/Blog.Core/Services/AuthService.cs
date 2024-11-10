using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blog.Core.Configurations;
using Blog.Core.Models;
using Blog.Core.Services.Interfaces;
using Blog.Data.Context;
using Blog.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly ApplicationDbContext _applicationDbContext;

        public AuthService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IOptions<JwtSettings> jwtSettings, ApplicationDbContext applicationDbContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<string> Register(RegisterUserViewModel registerUser)
        {
            string accessToken = string.Empty;

            if (await AuthorExists(registerUser.Email))
                throw new Exception("Usuário já cadastrado");

            var user = new IdentityUser
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                await CreateAuthor(registerUser, user);
                accessToken = await JwtGenerate(registerUser.Email);
            }

            return accessToken;
        }

        public async Task<IdentityResult> RegisterUserMvc(RegisterUserViewModel registerUser)
        {

            if (await AuthorExists(registerUser.Email))
                throw new Exception("Usuário já cadastrado");

            var user = new IdentityUser
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                await CreateAuthor(registerUser, user);
            }

            return result;
        }

        public async Task<string> Login(LoginUserViewModel loginUser)
        {
            var accessToken = string.Empty;
            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

            if (result.Succeeded)
            {
                accessToken = await JwtGenerate(loginUser.Email);
            }

            return accessToken;
        }

        public async Task<bool> AuthorExists(string email)
        {
            return await _applicationDbContext.Authors.AnyAsync(p => p.Email == email);
        }

        private async Task CreateAuthor(RegisterUserViewModel registerUser, IdentityUser user)
        {
            _applicationDbContext.Authors.Add(new Author
            {
                Id = user.Id,
                Email = user.Email,
                Name = registerUser.Name
            });

            await _applicationDbContext.SaveChangesAsync();
        }

        private async Task<string> JwtGenerate(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            foreach (var role in roles ?? Enumerable.Empty<string>())
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            return tokenHandler.WriteToken(token);
        }
    }
}