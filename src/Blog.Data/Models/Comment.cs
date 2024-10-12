using Microsoft.AspNetCore.Identity;
namespace Blog.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public virtual IdentityUser Author { get; set; }
        public string Content { get; set; }
        public int PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}