namespace Blog.Data.Models
{
    public class Comment : ModelBase
    {
        public string AuthorId { get; set; }
        public virtual Author Author { get; set; }
        public string Content { get; set; }
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}