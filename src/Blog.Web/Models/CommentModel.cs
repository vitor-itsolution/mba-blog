using System.ComponentModel.DataAnnotations;

namespace Blog.Web.Models
{
    public class CommentModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Display(Name = "Titulo")]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres")]
        public string Content { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Display(Name = "AutorId")]
        public Guid AuthorId { get; set; }
    }
}
