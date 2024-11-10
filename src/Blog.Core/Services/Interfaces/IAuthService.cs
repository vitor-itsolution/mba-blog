using Blog.Core.Models;

namespace Blog.Core.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> Register(RegisterUserViewModel registerUser);
        Task<string> Login(LoginUserViewModel loginUser);
    }
}