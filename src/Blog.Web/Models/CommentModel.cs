﻿using System.ComponentModel.DataAnnotations;

namespace Blog.Web.Models
{
    public class CommentModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PostId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Display(Name = "Comentário")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres")]
        public string Content { get; set; }

        [Display(Name = "Data")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        public string AuthorId { get; set; }

        [Display(Name = "Autor")]
        public string AuthorName { get; set; }
    }
}
