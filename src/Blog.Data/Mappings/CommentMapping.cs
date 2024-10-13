using Blog.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Data.Mappings
{
    public class CommentMapping : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Content)
            .IsRequired()
            .HasMaxLength(300);

            builder.HasOne(p => p.Author);

            builder.ToTable("Comments");
        }
    }
}