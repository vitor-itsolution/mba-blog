using Blog.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Blog.Core.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> Register(RegisterUserViewModel registerUser);
        Task<IdentityResult> RegisterUserMvc(RegisterUserViewModel registerUser);
        Task<string> Login(LoginUserViewModel loginUser);
        Task<bool> AuthorExists(string email);
    }
}