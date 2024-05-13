using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace NonHateSpeechForum.Data.Models
{
    public class Post
    {
        [Required]
        public Guid Id { get; set; }

        [Required] [MaxLength(3000)] 
        public string Content { get; set; } = null!;

        [Required] 
        public string AuthorId { get; set; } = null!;

        public IdentityUser? Author { get; set; }
    }
}