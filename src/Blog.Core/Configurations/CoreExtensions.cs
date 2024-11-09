using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return services.AddServices();
        }

        public static WebApplication AddWebApplicationConfigurations(this WebApplication app)
        {
            app.UseDbMigrationHelper();

            return app;
        }


        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();

            return services;
        }

        private static void UseDbMigrationHelper(this WebApplication app)
        {
            DbConfigurationHelper.EnsureSeedData(app).Wait();
        }
    }
}