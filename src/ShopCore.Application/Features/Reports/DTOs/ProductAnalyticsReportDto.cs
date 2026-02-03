namespace ShopCore.Application.Reports.DTOs;

public record ProductAnalyticsReportDto
{
    public int TotalProducts { get; init; }
    public int ActiveProducts { get; init; }
    public int OutOfStockProducts { get; init; }
    public List<TopProductDto> TopSellingProducts { get; init; } = new();
    public List<CategorySalesDto> SalesByCategory { get; init; } = new();
}

public record TopProductDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int SoldCount { get; init; }
    public decimal Revenue { get; init; }
}

public record CategorySalesDto
{
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public int ProductCount { get; init; }
    public decimal Revenue { get; init; }
}