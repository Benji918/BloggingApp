using BloggingApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BloggingApp.Data
{
    public class ApplicationDbcontext : IdentityDbContext
    {
        public DbSet<BlogModel> Blogs { get; set; }
        public DbSet<CommentsModel> Comments { get; set; }

        public ApplicationDbcontext(DbContextOptions<ApplicationDbcontext> options) : base(options)
        {

        }

    }
}
