using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Blog.Data.Models
{
    public class Author : ModelBase
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public virtual IdentityUser User { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}