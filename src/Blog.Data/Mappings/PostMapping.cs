using Blog.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Data.Mappings
{
    public class PostMapping : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Content)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(p => p.CreateDate);

            builder.HasOne(p => p.Author)
                   .WithMany()
                   .HasForeignKey(p => p.AuthorId);

            builder.HasMany(p => p.Comments)
                .WithOne(p => p.Post)
                .HasForeignKey(p => p.PostId);

            builder.ToTable("Posts");

        }
    }
}