using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopCore.Domain.Entities;
using ShopCore.Domain.Enums;

namespace ShopCore.Infrastructure.Data;

public class ApplicationDbContextSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationDbContextSeeder> _logger;

    public ApplicationDbContextSeeder(
        ApplicationDbContext context,
        ILogger<ApplicationDbContextSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // Seed Users
        if (!await _context.Users.AnyAsync())
        {
            await SeedUsersAsync();
            await _context.SaveChangesAsync();
        }

        // Seed Categories
        if (!await _context.Categories.AnyAsync())
        {
            await SeedCategoriesAsync();
            await _context.SaveChangesAsync();
        }

        // Seed Products (after categories and vendors)
        if (!await _context.Products.AnyAsync())
        {
            await SeedProductsAsync();
            await _context.SaveChangesAsync();
        }

        // Seed Coupons
        if (!await _context.Coupons.AnyAsync())
        {
            await SeedCouponsAsync();
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Database seeding completed successfully");
    }

    private async Task SeedUsersAsync()
    {
        _logger.LogInformation("Seeding users...");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123!", 12);

        var users = new List<User>
        {
            // Admin user
            new()
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@shopcore.com",
                PhoneNumber = "+919876543210",
                PasswordHash = passwordHash,
                Role = UserRole.Admin,
                IsEmailVerified = true,
                IsPhoneVerified = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            // Vendor user 1
            new()
            {
                FirstName = "Green",
                LastName = "Grocers",
                Email = "vendor@shopcore.com",
                PhoneNumber = "+919876543211",
                PasswordHash = passwordHash,
                Role = UserRole.Vendor,
                IsEmailVerified = true,
                IsPhoneVerified = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                VendorProfile = new VendorProfile
                {
                    BusinessName = "Green Grocers",
                    BusinessDescription = "Fresh organic vegetables and fruits delivered to your doorstep",
                    BusinessAddress = "123 Green Market, Bangalore 560001",
                    GstNumber = "29ABCDE1234F1ZK",
                    PanNumber = "ABCDE1234F",
                    BankName = "HDFC Bank",
                    BankAccountNumber = "12345678901234",
                    BankIfscCode = "HDFC0001234",
                    BankAccountHolderName = "Green Grocers Pvt Ltd",
                    Status = VendorStatus.Active,
                    CommissionRate = 10.0m,
                    ApprovedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                }
            },
            // Vendor user 2
            new()
            {
                FirstName = "Farm",
                LastName = "Fresh",
                Email = "farmfresh@shopcore.com",
                PhoneNumber = "+919876543212",
                PasswordHash = passwordHash,
                Role = UserRole.Vendor,
                IsEmailVerified = true,
                IsPhoneVerified = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                VendorProfile = new VendorProfile
                {
                    BusinessName = "Farm Fresh Dairy",
                    BusinessDescription = "Farm fresh milk, curd, paneer and dairy products",
                    BusinessAddress = "456 Dairy Lane, Bangalore 560002",
                    GstNumber = "29FGHIJ5678K2ZL",
                    PanNumber = "FGHIJ5678K",
                    BankName = "ICICI Bank",
                    BankAccountNumber = "98765432109876",
                    BankIfscCode = "ICIC0005678",
                    BankAccountHolderName = "Farm Fresh Dairy LLP",
                    Status = VendorStatus.Active,
                    CommissionRate = 8.0m,
                    ApprovedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                }
            },
            // Customer users
            new()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "+919876543213",
                PasswordHash = passwordHash,
                Role = UserRole.Customer,
                IsEmailVerified = true,
                IsPhoneVerified = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Cart = new Cart { CreatedAt = DateTime.UtcNow }
            },
            new()
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                PhoneNumber = "+919876543214",
                PasswordHash = passwordHash,
                Role = UserRole.Customer,
                IsEmailVerified = true,
                IsPhoneVerified = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Cart = new Cart { CreatedAt = DateTime.UtcNow }
            }
        };

        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        // Add addresses for customers
        var john = await _context.Users.FirstAsync(u => u.Email == "john@example.com");
        var jane = await _context.Users.FirstAsync(u => u.Email == "jane@example.com");

        var addresses = new List<Address>
        {
            new()
            {
                UserId = john.Id,
                User = john,
                Type = AddressType.Home,
                AddressLine1 = "123 Main Street",
                AddressLine2 = "Apartment 4B",
                City = "Bangalore",
                State = "Karnataka",
                Country = "India",
                Pincode = "560001",
                IsDefault = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                UserId = john.Id,
                User = john,
                Type = AddressType.Office,
                AddressLine1 = "456 Tech Park",
                AddressLine2 = "Building C",
                City = "Bangalore",
                State = "Karnataka",
                Country = "India",
                Pincode = "560100",
                IsDefault = false,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                UserId = jane.Id,
                User = jane,
                Type = AddressType.Home,
                AddressLine1 = "789 Park Avenue",
                City = "Bangalore",
                State = "Karnataka",
                Country = "India",
                Pincode = "560002",
                IsDefault = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await _context.Addresses.AddRangeAsync(addresses);

        // Add service areas for vendors
        var vendor1 = await _context.VendorProfiles.FirstAsync(v => v.BusinessName == "Green Grocers");
        var vendor2 = await _context.VendorProfiles.FirstAsync(v => v.BusinessName == "Farm Fresh Dairy");

        var serviceAreas = new List<VendorServiceArea>
        {
            new()
            {
                VendorId = vendor1.Id,
                AreaName = "Central Bangalore",
                City = "Bangalore",
                State = "Karnataka",
                Pincodes = new List<string> { "560001", "560002", "560003", "560004", "560005" },
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                VendorId = vendor1.Id,
                AreaName = "Electronic City",
                City = "Bangalore",
                State = "Karnataka",
                Pincodes = new List<string> { "560100", "560099", "560103" },
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                VendorId = vendor2.Id,
                AreaName = "Whitefield",
                City = "Bangalore",
                State = "Karnataka",
                Pincodes = new List<string> { "560066", "560067", "560001", "560002" },
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await _context.VendorServiceAreas.AddRangeAsync(serviceAreas);

        _logger.LogInformation("Seeded {Count} users with vendors, addresses and service areas", users.Count);
    }

    private async Task SeedCategoriesAsync()
    {
        _logger.LogInformation("Seeding categories...");

        var categories = new List<Category>
        {
            new()
            {
                Name = "Vegetables",
                Slug = "vegetables",
                Description = "Fresh organic vegetables",
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                SubCategories = new List<Category>
                {
                    new() { Name = "Leafy Greens", Slug = "leafy-greens", DisplayOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Root Vegetables", Slug = "root-vegetables", DisplayOrder = 2, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Gourds", Slug = "gourds", DisplayOrder = 3, IsActive = true, CreatedAt = DateTime.UtcNow }
                }
            },
            new()
            {
                Name = "Fruits",
                Slug = "fruits",
                Description = "Fresh seasonal fruits",
                DisplayOrder = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                SubCategories = new List<Category>
                {
                    new() { Name = "Citrus Fruits", Slug = "citrus-fruits", DisplayOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Tropical Fruits", Slug = "tropical-fruits", DisplayOrder = 2, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Berries", Slug = "berries", DisplayOrder = 3, IsActive = true, CreatedAt = DateTime.UtcNow }
                }
            },
            new()
            {
                Name = "Dairy",
                Slug = "dairy",
                Description = "Fresh dairy products",
                DisplayOrder = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                SubCategories = new List<Category>
                {
                    new() { Name = "Milk", Slug = "milk", DisplayOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Curd & Yogurt", Slug = "curd-yogurt", DisplayOrder = 2, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Cheese & Paneer", Slug = "cheese-paneer", DisplayOrder = 3, IsActive = true, CreatedAt = DateTime.UtcNow }
                }
            },
            new()
            {
                Name = "Groceries",
                Slug = "groceries",
                Description = "Daily essentials and groceries",
                DisplayOrder = 4,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                SubCategories = new List<Category>
                {
                    new() { Name = "Rice & Grains", Slug = "rice-grains", DisplayOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Pulses & Lentils", Slug = "pulses-lentils", DisplayOrder = 2, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new() { Name = "Spices", Slug = "spices", DisplayOrder = 3, IsActive = true, CreatedAt = DateTime.UtcNow }
                }
            }
        };

        await _context.Categories.AddRangeAsync(categories);
        _logger.LogInformation("Seeded {Count} categories", categories.Count);
    }

    private async Task SeedProductsAsync()
    {
        _logger.LogInformation("Seeding products...");

        var vendor1 = await _context.VendorProfiles.FirstAsync(v => v.BusinessName == "Green Grocers");
        var vendor2 = await _context.VendorProfiles.FirstAsync(v => v.BusinessName == "Farm Fresh Dairy");

        var leafyGreens = await _context.Categories.FirstAsync(c => c.Slug == "leafy-greens");
        var rootVegetables = await _context.Categories.FirstAsync(c => c.Slug == "root-vegetables");
        var citrusFruits = await _context.Categories.FirstAsync(c => c.Slug == "citrus-fruits");
        var milk = await _context.Categories.FirstAsync(c => c.Slug == "milk");
        var curdYogurt = await _context.Categories.FirstAsync(c => c.Slug == "curd-yogurt");
        var cheesePaneer = await _context.Categories.FirstAsync(c => c.Slug == "cheese-paneer");

        var products = new List<Product>
        {
            // Green Grocers - Vegetables
            new()
            {
                VendorId = vendor1.Id,
                CategoryId = leafyGreens.Id,
                Name = "Fresh Spinach (Palak)",
                Slug = "fresh-spinach-palak",
                Description = "Farm fresh organic spinach, perfect for sabzi and smoothies",
                ShortDescription = "Fresh organic spinach - 250g bunch",
                Price = 30,
                CompareAtPrice = 40,
                StockQuantity = 100,
                SKU = "VEG-SPN-001",
                Weight = 250,
                WeightUnit = "g",
                Status = ProductStatus.Active,
                IsFeatured = true,
                IsSubscriptionAvailable = true,
                SubscriptionDiscount = 10,
                CreatedAt = DateTime.UtcNow,
                Images = new List<ProductImage>
                {
                    new() { ImageUrl = "/images/products/spinach.jpg", DisplayOrder = 1, CreatedAt = DateTime.UtcNow }
                },
                Specifications = new List<ProductSpecification>
                {
                    new() { Name = "Type", Value = "Organic" },
                    new() { Name = "Origin", Value = "Local Farm" }
                }
            },
            new()
            {
                VendorId = vendor1.Id,
                CategoryId = leafyGreens.Id,
                Name = "Methi (Fenugreek Leaves)",
                Slug = "methi-fenugreek-leaves",
                Description = "Fresh fenugreek leaves, great for parathas and sabzi",
                ShortDescription = "Fresh methi leaves - 200g bunch",
                Price = 25,
                CompareAtPrice = 35,
                StockQuantity = 80,
                SKU = "VEG-MTH-001",
                Weight = 200,
                WeightUnit = "g",
                Status = ProductStatus.Active,
                IsFeatured = false,
                IsSubscriptionAvailable = true,
                SubscriptionDiscount = 10,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                VendorId = vendor1.Id,
                CategoryId = rootVegetables.Id,
                Name = "Fresh Carrots",
                Slug = "fresh-carrots",
                Description = "Sweet and crunchy organic carrots",
                ShortDescription = "Fresh carrots - 500g pack",
                Price = 45,
                CompareAtPrice = 55,
                StockQuantity = 120,
                SKU = "VEG-CRT-001",
                Weight = 500,
                WeightUnit = "g",
                Status = ProductStatus.Active,
                IsFeatured = true,
                IsSubscriptionAvailable = true,
                SubscriptionDiscount = 15,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                VendorId = vendor1.Id,
                CategoryId = citrusFruits.Id,
                Name = "Orange (Nagpur)",
                Slug = "orange-nagpur",
                Description = "Sweet and juicy Nagpur oranges",
                ShortDescription = "Fresh Nagpur oranges - 1kg",
                Price = 120,
                CompareAtPrice = 150,
                StockQuantity = 50,
                SKU = "FRT-ORG-001",
                Weight = 1000,
                WeightUnit = "g",
                Status = ProductStatus.Active,
                IsFeatured = true,
                IsSubscriptionAvailable = false,
                CreatedAt = DateTime.UtcNow
            },
            // Farm Fresh Dairy - Dairy Products
            new()
            {
                VendorId = vendor2.Id,
                CategoryId = milk.Id,
                Name = "Farm Fresh Milk",
                Slug = "farm-fresh-milk",
                Description = "Fresh cow milk delivered daily from local farm",
                ShortDescription = "Fresh milk - 500ml",
                Price = 35,
                CompareAtPrice = 40,
                StockQuantity = 200,
                SKU = "DRY-MLK-001",
                Weight = 500,
                WeightUnit = "ml",
                Status = ProductStatus.Active,
                IsFeatured = true,
                IsSubscriptionAvailable = true,
                SubscriptionDiscount = 5,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                VendorId = vendor2.Id,
                CategoryId = milk.Id,
                Name = "Farm Fresh Milk - 1 Liter",
                Slug = "farm-fresh-milk-1l",
                Description = "Fresh cow milk delivered daily from local farm",
                ShortDescription = "Fresh milk - 1 Liter",
                Price = 65,
                CompareAtPrice = 75,
                StockQuantity = 150,
                SKU = "DRY-MLK-002",
                Weight = 1000,
                WeightUnit = "ml",
                Status = ProductStatus.Active,
                IsFeatured = true,
                IsSubscriptionAvailable = true,
                SubscriptionDiscount = 5,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                VendorId = vendor2.Id,
                CategoryId = curdYogurt.Id,
                Name = "Fresh Dahi (Curd)",
                Slug = "fresh-dahi-curd",
                Description = "Thick and creamy homemade style curd",
                ShortDescription = "Fresh curd - 400g",
                Price = 45,
                CompareAtPrice = 55,
                StockQuantity = 100,
                SKU = "DRY-CRD-001",
                Weight = 400,
                WeightUnit = "g",
                Status = ProductStatus.Active,
                IsFeatured = false,
                IsSubscriptionAvailable = true,
                SubscriptionDiscount = 8,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                VendorId = vendor2.Id,
                CategoryId = cheesePaneer.Id,
                Name = "Fresh Paneer",
                Slug = "fresh-paneer",
                Description = "Soft and fresh cottage cheese made from pure milk",
                ShortDescription = "Fresh paneer - 200g",
                Price = 90,
                CompareAtPrice = 110,
                StockQuantity = 60,
                SKU = "DRY-PNR-001",
                Weight = 200,
                WeightUnit = "g",
                Status = ProductStatus.Active,
                IsFeatured = true,
                IsSubscriptionAvailable = true,
                SubscriptionDiscount = 10,
                CreatedAt = DateTime.UtcNow
            }
        };

        await _context.Products.AddRangeAsync(products);
        _logger.LogInformation("Seeded {Count} products", products.Count);
    }

    private async Task SeedCouponsAsync()
    {
        _logger.LogInformation("Seeding coupons...");

        var coupons = new List<Coupon>
        {
            new()
            {
                Code = "WELCOME10",
                Description = "10% off on your first order",
                Type = CouponType.Percentage,
                DiscountPercentage = 10,
                MinOrderValue = 200,
                MaxDiscount = 100,
                UsageLimit = 1000,
                UsageCount = 0,
                UsageLimitPerUser = 1,
                ValidFrom = DateTime.UtcNow,
                ValidUntil = DateTime.UtcNow.AddMonths(6),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Code = "FLAT50",
                Description = "Flat Rs. 50 off on orders above Rs. 500",
                Type = CouponType.FixedAmount,
                DiscountAmount = 50,
                MinOrderValue = 500,
                UsageLimit = 500,
                UsageCount = 0,
                UsageLimitPerUser = 2,
                ValidFrom = DateTime.UtcNow,
                ValidUntil = DateTime.UtcNow.AddMonths(3),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Code = "SUBSCRIBE15",
                Description = "15% off on subscription orders",
                Type = CouponType.Percentage,
                DiscountPercentage = 15,
                MinOrderValue = 300,
                MaxDiscount = 150,
                UsageLimit = 200,
                UsageCount = 0,
                UsageLimitPerUser = 1,
                ValidFrom = DateTime.UtcNow,
                ValidUntil = DateTime.UtcNow.AddMonths(2),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await _context.Coupons.AddRangeAsync(coupons);
        _logger.LogInformation("Seeded {Count} coupons", coupons.Count);
    }
}
