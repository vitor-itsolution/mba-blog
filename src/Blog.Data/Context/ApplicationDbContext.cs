using Blog.Data.Mappings;
using Blog.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Context;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        foreach (var property in builder.Model.GetEntityTypes().SelectMany(
                   e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
            property.SetColumnType("varchar(1000)");

        builder.ApplyConfiguration(new PostMapping());
        builder.ApplyConfiguration(new CommentMapping());
        builder.ApplyConfiguration(new AuthorMapping());

        base.OnModelCreating(builder);
    }
}
