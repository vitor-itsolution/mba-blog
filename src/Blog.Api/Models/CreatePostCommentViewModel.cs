using System.ComponentModel.DataAnnotations;

namespace Blog.Api.Models
{
    public class CreatePostCommentViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string PostId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Display(Name = "Comentário")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres")]
        public string Content { get; set; }
    }
}