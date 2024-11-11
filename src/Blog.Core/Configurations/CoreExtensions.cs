using Blog.Core.Services;
using Blog.Core.Services.Interfaces;
using Blog.Data.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Core.Configurations
{
    public static class CoreExtensions
    {
        public static IServiceCollection AddServicesConfigurations(this IServiceCollection services)
        {
            return services.AddServices()
                           .AddHttpContextAccessor();
        }

        public static WebApplication AddWebApplicationConfigurations(this WebApplication app)
        {
            DbConfigurationHelper.EnsureSeedData(app).Wait();
            return app;
        }


        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}