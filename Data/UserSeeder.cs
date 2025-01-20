using BloggingApp.Constants;
using Microsoft.AspNetCore.Identity;


namespace BloggingApp.Data
{
    public class UserSeeder
    {
        public static async Task SeedUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            await CreateUserWithRole(userManager, email: "admin@devspot.com", pwd: "Admin1234@", role: Role.ADMIN);
            await CreateUserWithRole(userManager, email: "author@devspot.com", pwd: "Author@1234", role: Role.AUTHOR);

        }

        public static async Task CreateUserWithRole(UserManager<IdentityUser> userManager, string email,
                                                    string pwd, string role)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser
                {
                    Email = email,
                    EmailConfirmed = false,
                    UserName = email,
                };

                var result = await userManager.CreateAsync(user, password: pwd);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role: Role.ADMIN);
                }
                else
                {
                    var errorMsg = string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                    throw new Exception($"Failed to create user with {user.Email}, Errors: {errorMsg}");
                }
            }
        }
    }
}
