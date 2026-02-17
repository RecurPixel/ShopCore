using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShopCore.Application.Common.Interfaces;
using ShopCore.Domain.Entities;
using ShopCore.Domain.Enums;
using ShopCore.Infrastructure.Data;

namespace ShopCore.UnitTests.Infrastructure;

public class ShopCoreWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"ShopCoreTestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Find and remove all EF Core registrations
            ServiceDescriptor[] efServiceDescriptors = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true ||
                    d.ImplementationType?.FullName?.Contains("SqlServer") == true)
                .ToArray();

            foreach (ServiceDescriptor descriptor in efServiceDescriptors)
            {
                services.Remove(descriptor);
            }

            // Remove existing DbContext and IApplicationDbContext
            ServiceDescriptor? dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ApplicationDbContext));
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            ServiceDescriptor? iDbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IApplicationDbContext));
            if (iDbContextDescriptor != null)
                services.Remove(iDbContextDescriptor);

            // Add InMemory database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });

            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<ApplicationDbContext>());
        });
    }

    public void SeedDatabase()
    {
        using IServiceScope scope = Services.CreateScope();
        ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        SeedTestData(db);
    }

    private static void SeedTestData(ApplicationDbContext db)
    {
        // Seed admin user
        if (!db.Users.Any(u => u.Email == "admin@shopcore.com"))
        {
            db.Users.Add(new User
            {
                Email = "admin@shopcore.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                FirstName = "Admin",
                LastName = "User",
                Role = UserRole.Admin,
                IsEmailVerified = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Seed test customer
        if (!db.Users.Any(u => u.Email == "customer@test.com"))
        {
            db.Users.Add(new User
            {
                Email = "customer@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
                FirstName = "Test",
                LastName = "Customer",
                Role = UserRole.Customer,
                IsEmailVerified = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Seed test vendor user
        if (!db.Users.Any(u => u.Email == "vendor@test.com"))
        {
            User vendorUser = new()
            {
                Email = "vendor@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Vendor@123"),
                FirstName = "Test",
                LastName = "Vendor",
                Role = UserRole.Vendor,
                IsEmailVerified = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            db.Users.Add(vendorUser);
            db.SaveChanges();

            // Create vendor profile
            db.VendorProfiles.Add(new VendorProfile
            {
                UserId = vendorUser.Id,
                BusinessName = "Test Vendor",
                BusinessAddress = "123 Test Street",
                GstNumber = "GST123456789",
                PanNumber = "PAN123456",
                BankName = "Test Bank",
                BankAccountNumber = "1234567890",
                BankIfscCode = "TEST0001234",
                BankAccountHolderName = "Test Vendor",
                Status = VendorStatus.Active,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Seed categories
        if (!db.Categories.Any())
        {
            db.Categories.AddRange(
                new Category { Name = "Fruits", Slug = "fruits", Description = "Fresh fruits", DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
                new Category { Name = "Vegetables", Slug = "vegetables", Description = "Fresh vegetables", DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
                new Category { Name = "Dairy", Slug = "dairy", Description = "Milk and dairy products", DisplayOrder = 3, CreatedAt = DateTime.UtcNow }
            );
        }

        db.SaveChanges();

        // Seed products (need vendor first)
        VendorProfile? vendor = db.VendorProfiles.FirstOrDefault();
        Category? category = db.Categories.FirstOrDefault();

        if (vendor != null && category != null && !db.Products.Any())
        {
            db.Products.AddRange(
                new Product
                {
                    VendorId = vendor.Id,
                    CategoryId = category.Id,
                    Name = "Test Apple",
                    Slug = "test-apple",
                    Description = "Fresh red apples",
                    Price = 100m,
                    StockQuantity = 100,
                    Status = ProductStatus.Active,
                    IsFeatured = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    VendorId = vendor.Id,
                    CategoryId = category.Id,
                    Name = "Test Banana",
                    Slug = "test-banana",
                    Description = "Fresh yellow bananas",
                    Price = 50m,
                    StockQuantity = 50,
                    Status = ProductStatus.Active,
                    CreatedAt = DateTime.UtcNow
                }
            );
            db.SaveChanges();
        }
    }
}
