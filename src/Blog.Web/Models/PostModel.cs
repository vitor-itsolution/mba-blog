using System.ComponentModel.DataAnnotations;

namespace Blog.Web.Models
{
    public class PostModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Display(Name = "Titulo")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres")]
        public string Title { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Display(Name = "Conteúdo")]
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres")]
        public string Content { get; set; }

        [Display(Name = "Data de Criação")]
        [DataType(DataType.DateTime, ErrorMessage = "O campo {0} não está em formato incorreto")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        public string AuthorId { get; set; }

        [Display(Name = "Autor")]
        public string AuthorName  { get; set; }

        [Display(Name = "Comentários")]
        public int AmountComment { get; set; }
    }
}