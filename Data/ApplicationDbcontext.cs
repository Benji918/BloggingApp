using BloggingApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
