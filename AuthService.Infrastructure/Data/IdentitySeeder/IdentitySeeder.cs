using AuthService.Domain.CommonFunctions;
using Microsoft.AspNetCore.Identity;
namespace AuthService.Infrastructure.Data.IdentitySeeder
{
    public static class IdentitySeeder
    {
        public static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles =
            {
            SD.Role_Admin,
            SD.Role_Individual
        };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
