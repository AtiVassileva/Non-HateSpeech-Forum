using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace NonHateSpeechForum.Data.Models
{
    using static Common.ValidationConstants;

    public class Post
    {
        [Required]
        public Guid Id { get; set; }

        [Required] [MaxLength(ContentMaxLength)] 
        public string Content { get; set; } = null!;

        [Required] 
        public string AuthorId { get; set; } = null!;

        public IdentityUser? Author { get; set; }
    }
}