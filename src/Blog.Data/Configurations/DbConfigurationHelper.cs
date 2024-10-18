using Blog.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blog.Data.Configurations
{
    public static class DbMigrationHelperExtension
    {
        public static void UseDbMigrationHelper(this WebApplication app)
        {
            DbConfigurationHelper.EnsureSeedData(app).Wait();
        }
    }
    public static class DbConfigurationHelper
    {
        public static async Task EnsureSeedData(WebApplication serviceScope)
        {
            var services = serviceScope.Services.CreateScope().ServiceProvider;
            await EnsureSeedData(services);
        }
        public static async Task EnsureSeedData(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (env.IsDevelopment())
            {
                await context.Database.MigrateAsync();
                await EnsureSeedUser(context);
            }
        }

        private static async Task EnsureSeedUser(ApplicationDbContext context)
        {
            if (context.Users.Any())
                return;
            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin@teste.com",
                NormalizedUserName = "admin@teste.com",
                Email = "admin@teste.com",
                NormalizedEmail = "admin@teste.com",
                AccessFailedCount = 0,
                LockoutEnabled = false,
                PasswordHash = "AQAAAAIAAYagAAAAECURsIAzQVesdThx4rPP3e6cRRmG1MNxzQWVJ+5RBlVXRzRywG//phfwDJ6esW2bNw==",
                TwoFactorEnabled = false,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var role = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Admin",
                NormalizedName = "Admin",
                ConcurrencyStamp = Guid.NewGuid().ToString()

            };

            var userRole = new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = role.Id
            };

            await context.Roles.AddAsync(role);
            await context.Users.AddAsync(user);
            await context.UserRoles.AddAsync(userRole);

            await context.SaveChangesAsync();

        }
    }
}