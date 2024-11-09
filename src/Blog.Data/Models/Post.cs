namespace Blog.Data.Models
{
    public class Post : ModelBase
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }
        public virtual Author Author { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = [];
    }
}