using Blog.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Data.Mappings
{
    public class PostMapping : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p=> p.Id);

            builder.Property(p=> p.Title)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p=> p.Content)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(p=> p.CreateDate);

            builder.HasOne(p => p.Author);

            builder.HasMany(p => p.Comments)
                .WithOne(p => p.Post)
                .HasForeignKey(p => p.PostId);

            builder.ToTable("Posts");
         
        }
    }
}