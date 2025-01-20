using Microsoft.AspNetCore.Identity;
using BloggingApp.Constants;
namespace BloggingApp.Data
{
    public class RoleSeeder
    {
        public static async Task SeedRoleAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(Role.ADMIN))
            {
                await roleManager.CreateAsync(new IdentityRole(Role.ADMIN));
            }

            if (!await roleManager.RoleExistsAsync(Role.AUTHOR))
            {
                await roleManager.CreateAsync(new IdentityRole(Role.AUTHOR));
            }
        }
    }
}
