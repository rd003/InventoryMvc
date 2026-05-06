using InventoryMvc.Constants;
using InventoryMvc.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryMvc.Extensions;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = app.Logger;
        try
        {

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            //await dbContext.Database.EnsureCreatedAsync();

            if (dbContext.Database.GetPendingMigrations().Any())
            {
                await dbContext.Database.MigrateAsync();
                logger.LogInformation("===Migrations are applied.===");
            }

            // seed roles

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in Roles.GetAll())
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger.LogInformation($"===Role: {role} is created.===");
                }
            }

            // seed admin (default user)
            if (!await dbContext.Users.AnyAsync())
            {
                var admin = new IdentityUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };

                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                if (userMgr is not null)
                {
                    await userMgr.CreateAsync(admin, "Admin@123");
                    await userMgr.AddToRoleAsync(admin, Roles.Admin);
                    logger.LogInformation($"===User: Admin is created.===");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database initialization failed");
            throw;
        }
    }
}
