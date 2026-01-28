namespace ShopCore.Application.Reports.DTOs;

public record ProductAnalyticsReportDto(
    int TotalProducts,
    int ActiveProducts,
    int OutOfStockProducts,
    List<TopProductDto> TopSellingProducts,
    List<CategorySalesDto> SalesByCategory
);

public record TopProductDto(
    int ProductId,
    string ProductName,
    int SoldCount,
    decimal Revenue
);

public record CategorySalesDto(
    int CategoryId,
    string CategoryName,
    int ProductCount,
    decimal Revenue
);
