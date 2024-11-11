using System.ComponentModel.DataAnnotations;

namespace Blog.Api.Models
{
    public class UpdateCommentViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Display(Name = "Comentário")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres")]
        public string Content { get; set; }
    }
}