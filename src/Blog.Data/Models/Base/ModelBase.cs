namespace Blog.Data.Models
{
    public class ModelBase
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreateDate { get; set; } = DateTime.Now;

    }
}