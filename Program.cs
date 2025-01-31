using BloggingApp.Data;
using BloggingApp.Models;
using BloggingApp.Repositories;
using BloggingApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using dotenv.net;
using Microsoft.Extensions.Configuration;

namespace BloggingApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotEnv.Load();
            var builder = WebApplication.CreateBuilder(args);

            // Add environment variables to configuration (including those loaded from .env)
            builder.Configuration.AddEnvironmentVariables();

            // Manually set configuration values from environment variables
            builder.Configuration["Smtp:Host"] = Environment.GetEnvironmentVariable("SMTP_HOST");
            builder.Configuration["Smtp:Port"] = Environment.GetEnvironmentVariable("SMTP_PORT");
            // Convert Port before binding
            var smtpPort = int.TryParse(builder.Configuration["Smtp:Port"], out var portValue) ? portValue : 587;

            builder.Configuration["Smtp:Port"] = smtpPort.ToString();
            builder.Configuration["Smtp:Username"] = Environment.GetEnvironmentVariable("SMTP_USERNAME");
            builder.Configuration["Smtp:Password"] = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
            builder.Configuration["Smtp:FromEmail"] = Environment.GetEnvironmentVariable("SMTP_FROM_EMAIL");
            //Console.WriteLine(builder.Configuration["Smtp:FromEmail"]);


            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
            // Add logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Services.AddDbContext<ApplicationDbcontext>(
                options =>
                {
                    options.UseNpgsql(builder.Configuration.GetConnectionString("DB"))
                     .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
                }
                );

            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbcontext>();

            builder.Services.AddScoped<IRepository<BlogModel>, BlogRepository>();
            builder.Services.AddScoped<IRepository<CommentsModel>, CommentRepository>();

            //Add email service
            builder.Services.AddTransient<IEmailSender, EmailSender>();

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
