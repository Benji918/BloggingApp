using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BloggingApp.Models;

namespace BloggingApp.Data
{
    public class ApplicationDbcontext : IdentityDbContext
    {
        DbSet<BlogModel> Blogs { get; set; }
        DbSet<CommentsModel> Comments { get; set; }

        public ApplicationDbcontext(DbContextOptions<ApplicationDbcontext> options) : base(options)
        {

        }

    }
}
