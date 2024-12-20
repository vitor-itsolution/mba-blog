﻿using System.ComponentModel.DataAnnotations;

namespace Blog.Core.Models
{
    public class CommentModel
    {
        [Key]
        public string Id { get; set; }
        public string PostId { get; set; }

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
