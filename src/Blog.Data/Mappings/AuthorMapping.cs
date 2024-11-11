using Blog.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Data.Mappings
{
    public class AuthorMapping : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.Email)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasOne(p => p.User)
                   .WithOne()
                   .HasForeignKey<Author>(p=> p.Id)
                   .IsRequired();

            builder.ToTable("Authors");
        }
    }
}