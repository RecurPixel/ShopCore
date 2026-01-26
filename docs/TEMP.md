AuthController

// Commands
public record RegisterUserCommand(string Email, string Password, string FirstName, string LastName, string PhoneNumber, UserRole Role) : IRequest<RegisterResponse>;
public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
public record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshTokenResponse>;
public record LogoutCommand : IRequest;
public record ForgotPasswordCommand(string Email) : IRequest;
public record ResetPasswordCommand(string Token, string NewPassword) : IRequest;
public record VerifyEmailCommand(string Token) : IRequest;

// Queries
public record GetCurrentUserQuery : IRequest<UserProfileDto>;


UsersController
// Commands
public record UpdateUserCommand(string FirstName, string LastName, string PhoneNumber) : IRequest<UserProfileDto>;
public record UploadAvatarCommand(IFormFile File) : IRequest<string>;
public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest;

// Queries
public record GetUserProfileQuery : IRequest<UserProfileDto>;

AddressesController
// Commands
public record CreateAddressCommand(string FullName, string PhoneNumber, string AddressLine1, string? AddressLine2, string City, string State, string Pincode) : IRequest<AddressDto>;
public record UpdateAddressCommand(int Id, string FullName, string PhoneNumber, string AddressLine1, string? AddressLine2, string City, string State, string Pincode) : IRequest<AddressDto>;
public record DeleteAddressCommand(int Id) : IRequest;
public record SetDefaultAddressCommand(int Id) : IRequest;

// Queries
public record GetMyAddressesQuery : IRequest<List<AddressDto>>;
public record GetAddressByIdQuery(int Id) : IRequest<AddressDto>;


VendorsController
// Commands
public record RegisterVendorCommand(string BusinessName, string BusinessDescription, string BusinessAddress, string GstNumber, string PanNumber, string BankName, string BankAccountNumber, string BankIfscCode, string BankAccountHolderName) : IRequest<VendorProfileDto>;
public record UpdateVendorProfileCommand(string BusinessName, string BusinessDescription, string BusinessAddress, string BankName, string BankAccountNumber, string BankIfscCode, string BankAccountHolderName) : IRequest<VendorProfileDto>;
public record UploadVendorLogoCommand(IFormFile File) : IRequest<string>;

// Admin Commands
public record ApproveVendorCommand(int VendorId) : IRequest;
public record SuspendVendorCommand(int VendorId, string Reason) : IRequest;

// Queries
public record GetMyVendorProfileQuery : IRequest<VendorProfileDto>;
public record GetVendorStatsQuery : IRequest<VendorStatsDto>;
public record GetVendorOrdersQuery(int Page = 1, int PageSize = 20, OrderStatus? Status = null) : IRequest<PaginatedList<VendorOrderDto>>;
public record GetPendingVendorsQuery : IRequest<List<VendorProfileDto>>;


CategoriesController
// Commands (Admin only)
public record CreateCategoryCommand(string Name, string Description, int? ParentCategoryId, int DisplayOrder) : IRequest<CategoryDto>;
public record UpdateCategoryCommand(int Id, string Name, string Description, int? ParentCategoryId, int DisplayOrder) : IRequest<CategoryDto>;
public record DeleteCategoryCommand(int Id) : IRequest;
public record UploadCategoryImageCommand(int Id, IFormFile File) : IRequest<string>;

// Queries
public record GetCategoriesQuery : IRequest<List<CategoryDto>>;
public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto>;
public record GetProductsByCategoryQuery(int CategoryId, int Page = 1, int PageSize = 20) : IRequest<PaginatedList<ProductDto>>;



ProductsController
// Commands (Vendor)
public record CreateProductCommand(string Name, string Description, decimal Price, decimal? CompareAtPrice, int CategoryId, int StockQuantity, string? SKU, bool IsSubscriptionAvailable, decimal? SubscriptionDiscount) : IRequest<ProductDto>;
public record UpdateProductCommand(int Id, string Name, string Description, decimal Price, decimal? CompareAtPrice, int CategoryId, int StockQuantity, string? SKU, bool IsSubscriptionAvailable, decimal? SubscriptionDiscount) : IRequest<ProductDto>;
public record DeleteProductCommand(int Id) : IRequest;
public record UploadProductImagesCommand(int ProductId, List<IFormFile> Files) : IRequest<List<ProductImageDto>>;
public record DeleteProductImageCommand(int ProductId, int ImageId) : IRequest;
public record UpdateProductStatusCommand(int Id, ProductStatus Status) : IRequest;
public record AddProductSpecificationCommand(int ProductId, string Name, string Value) : IRequest;

// Queries
public record GetProductsQuery(int Page = 1, int PageSize = 20, int? CategoryId = null, int? VendorId = null, decimal? MinPrice = null, decimal? MaxPrice = null, bool? InStock = null, string? SortBy = null) : IRequest<PaginatedList<ProductDto>>;
public record GetProductByIdQuery(int Id) : IRequest<ProductDetailDto>;
public record SearchProductsQuery(string SearchTerm, int Page = 1, int PageSize = 20) : IRequest<PaginatedList<ProductDto>>;
public record GetFeaturedProductsQuery(int Count = 10) : IRequest<List<ProductDto>>;


CartController
// Commands
public record AddToCartCommand(int ProductId, int Quantity) : IRequest<CartDto>;
public record UpdateCartItemCommand(int CartItemId, int Quantity) : IRequest<CartDto>;
public record RemoveFromCartCommand(int CartItemId) : IRequest<CartDto>;
public record ClearCartCommand : IRequest;
public record ValidateCartCommand : IRequest<CartValidationResultDto>;

// Queries
public record GetCartQuery : IRequest<CartDto>;


OrdersController
// Commands
public record CreateOrderCommand(int AddressId, PaymentMethod PaymentMethod, string? CouponCode) : IRequest<OrderDto>;
public record CancelOrderCommand(int OrderId, string Reason) : IRequest;
public record UpdateOrderStatusCommand(int OrderId, OrderStatus Status) : IRequest;

// Queries
public record GetMyOrdersQuery(int Page = 1, int PageSize = 20, OrderStatus? Status = null, DateTime? FromDate = null, DateTime? ToDate = null) : IRequest<PaginatedList<OrderDto>>;
public record GetOrderByIdQuery(int OrderId) : IRequest<OrderDetailDto>;
public record GetOrderInvoiceQuery(int OrderId) : IRequest<byte[]>;


PaymentsController
// Commands
public record CreatePaymentIntentCommand(int OrderId) : IRequest<PaymentIntentDto>;
public record ConfirmPaymentCommand(string PaymentIntentId, string TransactionId) : IRequest<PaymentConfirmationDto>;

// Queries
public record GetPaymentHistoryQuery(int Page = 1, int PageSize = 20) : IRequest<PaginatedList<PaymentHistoryDto>>;



ReviewsController
// Commands
public record CreateReviewCommand(int ProductId, int Rating, string? Title, string? Comment, List<IFormFile>? Images) : IRequest<ReviewDto>;
public record UpdateReviewCommand(int Id, int Rating, string? Title, string? Comment) : IRequest<ReviewDto>;
public record DeleteReviewCommand(int Id) : IRequest;
public record MarkReviewHelpfulCommand(int ReviewId) : IRequest;
public record AddVendorResponseCommand(int ReviewId, string Response) : IRequest;

// Queries
public record GetProductReviewsQuery(int ProductId, int Page = 1, int PageSize = 20, string? SortBy = null) : IRequest<PaginatedList<ReviewDto>>;
public record GetMyReviewsQuery(int Page = 1, int PageSize = 20) : IRequest<PaginatedList<ReviewDto>>;


WishlistController
// Commands
public record AddToWishlistCommand(int ProductId) : IRequest;
public record RemoveFromWishlistCommand(int ProductId) : IRequest;
public record MoveToCartCommand(int ProductId) : IRequest<CartDto>;

// Queries
public record GetWishlistQuery : IRequest<WishlistDto>;



CouponsController
// Commands (Admin)
public record CreateCouponCommand(string Code, CouponType Type, decimal? DiscountPercentage, decimal? DiscountAmount, decimal? MinOrderValue, decimal? MaxDiscount, DateTime ValidFrom, DateTime ValidUntil, int? UsageLimit, int? UsageLimitPerUser) : IRequest<CouponDto>;
public record UpdateCouponCommand(int Id, string Code, CouponType Type, decimal? DiscountPercentage, decimal? DiscountAmount, decimal? MinOrderValue, decimal? MaxDiscount, DateTime ValidFrom, DateTime ValidUntil, int? UsageLimit, int? UsageLimitPerUser) : IRequest<CouponDto>;
public record DeactivateCouponCommand(int Id) : IRequest;

// Commands (Customer)
public record ValidateCouponCommand(string Code, decimal OrderTotal) : IRequest<CouponValidationResultDto>;

// Queries
public record GetActiveCouponsQuery : IRequest<List<CouponDto>>;
public record GetAllCouponsQuery : IRequest<List<CouponDto>>;


PayoutsController
// Commands (Admin)
public record CalculatePayoutsCommand(DateTime PeriodStart, DateTime PeriodEnd) : IRequest<List<VendorPayoutDto>>;
public record ProcessPayoutCommand(int PayoutId, string TransactionId) : IRequest;

// Queries (Vendor)
public record GetVendorPayoutsQuery(int Page = 1, int PageSize = 20) : IRequest<PaginatedList<VendorPayoutDto>>;
public record GetPendingPayoutQuery : IRequest<VendorPayoutDto>;



SubscriptionsController (PRIVATE)
// Commands
public record CreateSubscriptionCommand(int VendorId, int DeliveryAddressId, List<SubscriptionItemInput> Items, SubscriptionFrequency Frequency, int? CustomFrequencyDays, DateTime StartDate, string? PreferredDeliveryTime, int BillingCycleDays, decimal? DepositAmount) : IRequest<SubscriptionDto>;
public record UpdateSubscriptionCommand(int Id, List<SubscriptionItemInput> Items, string? PreferredDeliveryTime) : IRequest<SubscriptionDto>;
public record PauseSubscriptionCommand(int Id) : IRequest;
public record ResumeSubscriptionCommand(int Id) : IRequest;
public record CancelSubscriptionCommand(int Id, string Reason) : IRequest;
public record SettleSubscriptionCommand(int Id) : IRequest<SubscriptionSettlementDto>;

// Queries
public record GetMySubscriptionsQuery(SubscriptionStatus? Status = null, int Page = 1, int PageSize = 20) : IRequest<PaginatedList<SubscriptionDto>>;
public record GetSubscriptionByIdQuery(int Id) : IRequest<SubscriptionDto>;
public record GetVendorSubscriptionsQuery(int Page = 1, int PageSize = 20) : IRequest<PaginatedList<SubscriptionDto>>;




DeliveriesController (PRIVATE)
// Commands (Vendor/Driver)
public record CompleteDeliveryCommand(int Id, List<DeliveryItemStatusInput> Items, string? PaymentMethod, string? PaymentTransactionId) : IRequest;
public record MarkDeliveryFailedCommand(int Id, string Reason) : IRequest;

// Commands (Customer)
public record SkipDeliveryCommand(int Id, string Reason) : IRequest;

// Queries
public record GetSubscriptionDeliveriesQuery(int SubscriptionId, int Page = 1, int PageSize = 20) : IRequest<PaginatedList<DeliveryDto>>;
public record GetDeliveryByIdQuery(int Id) : IRequest<DeliveryDto>;
public record GetUpcomingDeliveriesQuery(DateTime? Date = null) : IRequest<List<DeliveryDto>>;



InvoicesController (PRIVATE)
// Commands (Vendor)
public record GenerateInvoiceCommand(int SubscriptionId) : IRequest<InvoiceDto>;

// Commands (Customer)
public record PayInvoiceCommand(int InvoiceId, string PaymentMethod, string TransactionId) : IRequest<PaymentConfirmationDto>;

// Queries
public record GetSubscriptionInvoicesQuery(int SubscriptionId, int Page = 1, int PageSize = 20) : IRequest<PaginatedList<InvoiceDto>>;
public record GetInvoiceByIdQuery(int Id) : IRequest<InvoiceDto>;
public record DownloadInvoiceQuery(int Id) : IRequest<byte[]>;
public record GetUnpaidInvoicesQuery : IRequest<List<InvoiceDto>>;


AdminController
// Queries
public record GetDashboardStatsQuery : IRequest<AdminDashboardStatsDto>;
public record GetRecentOrdersQuery(int Count = 10) : IRequest<List<RecentOrderDto>>;
public record GetRevenueStatsQuery(DateTime FromDate, DateTime ToDate) : IRequest<RevenueStatsDto>;



Helper Input Models
// For multi-item subscriptions
public record SubscriptionItemInput(int ProductId, int Quantity, decimal? DiscountPercentage);

// For delivery completion
public record DeliveryItemStatusInput(int DeliveryItemId, DeliveryItemStatus Status, string? Notes);


