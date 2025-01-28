using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace BloggingApp.Models
{
    public class BlogModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is Required!")]
        [StringLength(100, ErrorMessage = "Title must be less than 100 characters")]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public List<string> Author { get; set; } = new List<string>();

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string ImageUrl { get; set; } // Store the image file path or URL

        public ICollection<CommentsModel>? Comments { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [NotMapped]
        public IdentityUser User { get; set; }

    }
}
