using Microsoft.AspNetCore.Identity;

namespace Blog.Data.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public virtual IdentityUser Author { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = [];
    }
}