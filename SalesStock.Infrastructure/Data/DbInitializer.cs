using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SalesStock.Infrastructure.Identity;
using SalesStock.Infrastructure.Persistence;

namespace SalesStock.Infrastructure.Data
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();
            var env = Environment.GetEnvironmentVariable("RESET_DB");

            if (env == "true")
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
                await SeedRolesAndAdminAsync(host);
                return;
            }
            var dbCreated = await context.Database.EnsureCreatedAsync();
            if (dbCreated)
            {
                await context.Database.MigrateAsync();
                await SeedRolesAndAdminAsync(host);
            }
        }

        private static async Task SeedRolesAndAdminAsync(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "Manager", "Operator" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var adminUser = await userManager.FindByEmailAsync("admin@example.com");
            if (adminUser == null)
            {
                var newAdminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(newAdminUser, "Admin123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(newAdminUser, "Admin");
            }
        }
    }
}
