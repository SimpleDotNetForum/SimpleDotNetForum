using System.ComponentModel.DataAnnotations;

namespace SimpleDotNetForum.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "The title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "The content cannot exceed 500 characters.")]
        public string Content { get; set; }

        [Required]
        public string Thread { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<Reply> Replies { get; set; } = new List<Reply>();
    }
}
