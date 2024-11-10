using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Core.Models;

namespace Blog.Core.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> Register(RegisterUserViewModel registerUser);
        Task<string> Login(LoginUserViewModel loginUser);
    }
}