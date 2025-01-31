using BloggingApp.Data;
using BloggingApp.Models;
using BloggingApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BloggingApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbcontext>(
                options =>
                {
                    options.UseNpgsql(builder.Configuration.GetConnectionString("DB"))
                     .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
                }
                );

            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbcontext>();

            builder.Services.AddScoped<IRepository<BlogModel>, BlogRepository>();
            builder.Services.AddScoped<IRepository<CommentsModel>, CommentRepository>();


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5194); // HTTP
                options.ListenAnyIP(7112, listenOptions => listenOptions.UseHttps()); // HTTPS
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                RoleSeeder.SeedRoleAsync(services).Wait();
                UserSeeder.SeedUserAsync(services).Wait();

            }


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            // to use razor endpoints(login and register for scaffolded item)
            app.MapRazorPages();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.UseStaticFiles(); // Enable static file serving
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Blog}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
