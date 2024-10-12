using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Blog.Data.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public int AuthorId { get; set; }
        public virtual IdentityUser Author { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = [];
    }
}