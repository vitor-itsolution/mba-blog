using Blog.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Data.Mappings
{
    public class CommentMapping : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.Property(p => p.Id)
                .UseIdentityColumn(seed: 1, increment: 1)
                .IsRequired();

            builder.Property(p => p.Content)
            .IsRequired()
            .HasMaxLength(300);

            builder.HasOne(p => p.Author)
            .WithMany()
            .HasForeignKey(p => p.AuthorId);

            builder.ToTable("Comments");
        }
    }
}