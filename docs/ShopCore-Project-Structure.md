# ShopCore - Project Structure

**Architecture:** Clean Architecture with CQRS  
**Pattern:** Vertical Slice per Feature  
**Framework:** .NET 8

---

## Solution Structure

```
ShopCore/
├── src/
│   ├── ShopCore.API/              # Web API Layer
│   ├── ShopCore.Application/      # Business Logic Layer
│   ├── ShopCore.Domain/           # Domain Entities Layer
│   └── ShopCore.Infrastructure/   # Data Access Layer
├── tests/
│   ├── ShopCore.UnitTests/
│   └── ShopCore.IntegrationTests/
├── ShopCore.sln
└── README.md
```

---

## ShopCore.API (Presentation Layer)

```
ShopCore.API/
├── Controllers/                # API Controllers
│   ├── AuthController.cs
│   ├── ProductsController.cs
│   ├── OrdersController.cs
│   ├── CartsController.cs
│   └── SubscriptionsController.cs
├── Middleware/                 # Custom Middleware
│   ├── ExceptionHandlingMiddleware.cs
│   └── RequestLoggingMiddleware.cs
├── Filters/                    # Action Filters
│   └── ValidationFilter.cs
├── Extensions/                 # Service Extensions
│   ├── ServiceCollectionExtensions.cs
│   └── ApplicationBuilderExtensions.cs
├── appsettings.json
├── appsettings.Development.json
├── appsettings.Production.json
├── Program.cs                  # Entry Point
└── ShopCore.API.csproj
```

**Program.cs Structure:**
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application layer
builder.Services.AddApplication();

// Infrastructure layer
builder.Services.AddInfrastructure(builder.Configuration);

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* config */ });

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

**Controller Example:**
```csharp
[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] GetProductsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        
        if (product == null)
            return NotFound();
        
        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductCommand command)
    {
        var product = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetProduct), 
            new { id = product.Id }, 
            product);
    }
}
```

---

## ShopCore.Application (Business Logic Layer)

```
ShopCore.Application/
├── Common/                     # Shared Application Code
│   ├── Interfaces/
│   │   ├── IApplicationDbContext.cs
│   │   ├── ICurrentUserService.cs
│   │   └── IDateTime.cs
│   ├── Behaviours/             # MediatR Pipeline Behaviors
│   │   ├── ValidationBehaviour.cs
│   │   └── LoggingBehaviour.cs
│   ├── Exceptions/
│   │   ├── NotFoundException.cs
│   │   └── ValidationException.cs
│   └── Models/
│       └── PaginatedList.cs
│
├── Features/                   # Feature Folders (Vertical Slices)
│   ├── Auth/
│   │   ├── Commands/
│   │   │   ├── Register/
│   │   │   │   ├── RegisterCommand.cs
│   │   │   │   ├── RegisterCommandHandler.cs
│   │   │   │   └── RegisterCommandValidator.cs
│   │   │   └── Login/
│   │   │       ├── LoginCommand.cs
│   │   │       ├── LoginCommandHandler.cs
│   │   │       └── LoginCommandValidator.cs
│   │   └── Queries/
│   │
│   ├── Products/
│   │   ├── Commands/
│   │   │   ├── CreateProduct/
│   │   │   │   ├── CreateProductCommand.cs
│   │   │   │   ├── CreateProductCommandHandler.cs
│   │   │   │   └── CreateProductCommandValidator.cs
│   │   │   ├── UpdateProduct/
│   │   │   └── DeleteProduct/
│   │   └── Queries/
│   │       ├── GetProducts/
│   │       │   ├── GetProductsQuery.cs
│   │       │   ├── GetProductsQueryHandler.cs
│   │       │   └── ProductDto.cs
│   │       └── GetProductById/
│   │
│   ├── Orders/
│   │   ├── Commands/
│   │   │   ├── CreateOrder/
│   │   │   └── CancelOrder/
│   │   └── Queries/
│   │       └── GetOrders/
│   │
│   ├── Cart/
│   │   ├── Commands/
│   │   │   ├── AddToCart/
│   │   │   └── UpdateCartItem/
│   │   └── Queries/
│   │       └── GetCart/
│   │
│   ├── Subscriptions/          # PRIVATE
│   │   ├── Commands/
│   │   │   ├── CreateSubscription/
│   │   │   ├── PauseSubscription/
│   │   │   └── SettleSubscription/
│   │   └── Queries/
│   │       └── GetSubscriptions/
│   │
│   └── Invoices/               # PRIVATE
│       ├── Commands/
│       │   └── GenerateInvoice/
│       └── Queries/
│           └── GetInvoices/
│
├── DependencyInjection.cs      # AddApplication() extension
└── ShopCore.Application.csproj
```

**Feature Structure Example:**
```
Products/
├── Commands/
│   └── CreateProduct/
│       ├── CreateProductCommand.cs          # Command definition
│       ├── CreateProductCommandHandler.cs   # Business logic
│       └── CreateProductCommandValidator.cs # Validation rules
└── Queries/
    └── GetProducts/
        ├── GetProductsQuery.cs              # Query definition
        ├── GetProductsQueryHandler.cs       # Data retrieval
        └── ProductDto.cs                    # Response model
```

**Command Example:**
```csharp
// CreateProductCommand.cs
public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int CategoryId,
    int StockQuantity
) : IRequest<ProductDto>;

// CreateProductCommandHandler.cs
public class CreateProductCommandHandler 
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateProductCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ProductDto> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CategoryId = request.CategoryId,
            StockQuantity = request.StockQuantity,
            VendorId = _currentUser.VendorId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };
    }
}

// CreateProductCommandValidator.cs
public class CreateProductCommandValidator 
    : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0);
    }
}
```

**Query Example:**
```csharp
// GetProductsQuery.cs
public record GetProductsQuery(
    int Page = 1,
    int PageSize = 20,
    int? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null
) : IRequest<PaginatedList<ProductDto>>;

// GetProductsQueryHandler.cs
public class GetProductsQueryHandler 
    : IRequestHandler<GetProductsQuery, PaginatedList<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<ProductDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Products.AsQueryable();

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId);

        if (request.MinPrice.HasValue)
            query = query.Where(p => p.Price >= request.MinPrice);

        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= request.MaxPrice);

        return await query
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            })
            .ToPaginatedListAsync(request.Page, request.PageSize);
    }
}

// ProductDto.cs
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string CategoryName { get; set; }
}
```

**DependencyInjection.cs:**
```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(
                Assembly.GetExecutingAssembly()));

        // FluentValidation
        services.AddValidatorsFromAssembly(
            Assembly.GetExecutingAssembly());

        // Pipeline Behaviors
        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehaviour<,>));

        // AutoMapper
        services.AddAutoMapper(
            Assembly.GetExecutingAssembly());

        return services;
    }
}
```

---

## ShopCore.Domain (Domain Layer)

```
ShopCore.Domain/
├── Entities/                   # Domain Entities
│   ├── User.cs
│   ├── VendorProfile.cs
│   ├── Address.cs
│   ├── Category.cs
│   ├── Product.cs
│   ├── ProductImage.cs
│   ├── Cart.cs
│   ├── CartItem.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   ├── Review.cs
│   ├── Coupon.cs
│   ├── Subscription.cs         # PRIVATE
│   ├── Delivery.cs             # PRIVATE
│   └── SubscriptionInvoice.cs  # PRIVATE
│
├── Enums/                      # Enumerations
│   ├── UserRole.cs
│   ├── OrderStatus.cs
│   ├── PaymentStatus.cs
│   ├── SubscriptionStatus.cs
│   └── DeliveryStatus.cs
│
├── Common/                     # Base Classes
│   ├── BaseEntity.cs
│   └── AuditableEntity.cs
│
└── ShopCore.Domain.csproj
```

**Entity Example:**
```csharp
// Product.cs
public class Product : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int StockQuantity { get; set; }
    public string SKU { get; set; }
    
    // Relationships
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    
    public int VendorId { get; set; }
    public VendorProfile Vendor { get; set; }
    
    // Collections
    public ICollection<ProductImage> Images { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<CartItem> CartItems { get; set; }
    
    // Subscription
    public bool IsSubscriptionAvailable { get; set; }
    public decimal? SubscriptionDiscount { get; set; }
}

// AuditableEntity.cs
public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}
```

---

## ShopCore.Infrastructure (Data Layer)

```
ShopCore.Infrastructure/
├── Data/                       # Database Context
│   ├── ApplicationDbContext.cs
│   ├── Configurations/         # Entity Configurations
│   │   ├── UserConfiguration.cs
│   │   ├── ProductConfiguration.cs
│   │   ├── OrderConfiguration.cs
│   │   └── SubscriptionConfiguration.cs
│   └── Migrations/             # EF Migrations
│
├── Services/                   # Infrastructure Services
│   ├── CurrentUserService.cs
│   ├── DateTimeService.cs
│   ├── EmailService.cs
│   └── FileStorageService.cs
│
├── Identity/                   # Authentication
│   ├── JwtTokenService.cs
│   └── PasswordHasher.cs
│
├── Repositories/               # (Optional - if not using DbContext directly)
│   └── Repository.cs
│
├── DependencyInjection.cs      # AddInfrastructure() extension
└── ShopCore.Infrastructure.csproj
```

**ApplicationDbContext.cs:**
```csharp
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<VendorProfile> VendorProfiles { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Review> Reviews { get; set; }
    
    // Subscription tables (PRIVATE)
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Delivery> Deliveries { get; set; }
    public DbSet<SubscriptionInvoice> SubscriptionInvoices { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        // Set audit fields
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

**Entity Configuration Example:**
```csharp
// ProductConfiguration.cs
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.HasIndex(p => p.Slug)
            .IsUnique();

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Vendor)
            .WithMany(v => v.Products)
            .HasForeignKey(p => p.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
```

**DependencyInjection.cs:**
```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IEmailService, EmailService>();

        // Authentication
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
```

---

## Tests

```
ShopCore.UnitTests/
├── Application/
│   └── Features/
│       ├── Products/
│       │   └── Commands/
│       │       └── CreateProductCommandTests.cs
│       └── Orders/
└── ShopCore.UnitTests.csproj

ShopCore.IntegrationTests/
├── Controllers/
│   ├── ProductsControllerTests.cs
│   └── OrdersControllerTests.cs
├── CustomWebApplicationFactory.cs
└── ShopCore.IntegrationTests.csproj
```

**Unit Test Example:**
```csharp
public class CreateProductCommandTests
{
    [Fact]
    public async Task Handle_ValidCommand_CreatesProduct()
    {
        // Arrange
        var command = new CreateProductCommand(
            "Test Product",
            "Description",
            100.00m,
            1,
            10
        );

        var handler = new CreateProductCommandHandler(
            _contextMock.Object,
            _currentUserMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Product");
    }
}
```

---

## NuGet Packages Required

**ShopCore.API:**
```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
<PackageReference Include="Swashbuckle.AspNetCore" />
```

**ShopCore.Application:**
```xml
<PackageReference Include="MediatR" />
<PackageReference Include="FluentValidation" />
<PackageReference Include="AutoMapper" />
```

**ShopCore.Infrastructure:**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" />
```

**Tests:**
```xml
<PackageReference Include="xUnit" />
<PackageReference Include="Moq" />
<PackageReference Include="FluentAssertions" />
```

---

## Key Principles

1. **Separation of Concerns** - Each layer has single responsibility
2. **Dependency Rule** - Dependencies point inward (Domain has no dependencies)
3. **Feature Folders** - Organize by feature, not technical concern
4. **CQRS** - Separate read (queries) from write (commands)
5. **MediatR** - Decouples controllers from business logic
6. **Validation** - FluentValidation in application layer
7. **DTOs** - Never expose entities directly

---

## Quick Commands

```bash
# Create migration
dotnet ef migrations add InitialCreate --project ShopCore.Infrastructure --startup-project ShopCore.API

# Update database
dotnet ef database update --project ShopCore.Infrastructure --startup-project ShopCore.API

# Run API
dotnet run --project ShopCore.API

# Run tests
dotnet test
```
