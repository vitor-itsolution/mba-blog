using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Configurations;
using Blog.Core.Models;
using Blog.Core.Services.Interfaces;
using Blog.Data.Context;
using Blog.Data.Models;
using Microsoft.AspNetCore.Identity;
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
                accessToken = JwtGenerate();
            }

            return accessToken;
        }

        public async Task<string> Login(LoginUserViewModel loginUser)
        {
            var accessToken = string.Empty;
            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

            if (result.Succeeded)
            {
                accessToken = JwtGenerate();
            }

            return accessToken;
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

        private string JwtGenerate()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            return tokenHandler.WriteToken(token);
        }
    }
}