using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace BloggingApp.Models;

public class CommentsModel
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string Comment { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [Required]
    public DateTime UpdatedDate { get; set; } = DateTime.Now;

    [Required]
    public string UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public IdentityUser User { get; set; }

    [Required]
    public int BlogId { get; set; }

    [ForeignKey(nameof(BlogId))]
    public BlogModel Blog { get; set; }
}