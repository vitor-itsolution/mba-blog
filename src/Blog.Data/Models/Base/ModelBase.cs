namespace Blog.Data.Models
{
    public class ModelBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreateDate { get; set; } = DateTime.Now;

    }
}