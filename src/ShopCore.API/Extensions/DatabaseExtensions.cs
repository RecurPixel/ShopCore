using ShopCore.Infrastructure.Data;

namespace ShopCore.API.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();

            // MigrateAsync only works with relational providers (SQL Server, Postgres).
            // In-memory database (used by integration tests) skips this.
            if (context.Database.IsRelational())
            {
                logger.LogInformation("Applying database migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully");
            }

            if (app.Environment.IsDevelopment())
            {
                logger.LogInformation("Seeding database...");
                var seeder = services.GetRequiredService<ApplicationDbContextSeeder>();
                await seeder.SeedAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating or seeding the database");
            throw;
        }
    }
}
