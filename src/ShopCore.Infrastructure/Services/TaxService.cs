using Microsoft.Extensions.Configuration;
using ShopCore.Application.Common.Interfaces;

namespace ShopCore.Infrastructure.Services;

/// <summary>
/// Service for calculating tax amounts based on configured tax rate
/// </summary>
public class TaxService : ITaxService
{
    private readonly decimal _taxRate;
    private readonly int _roundToDecimalPlaces;

    public TaxService(IConfiguration configuration)
    {
        _taxRate = configuration.GetValue<decimal>("TaxSettings:GSTRate", 0.18m);
        _roundToDecimalPlaces = configuration.GetValue<int>("TaxSettings:RoundToDecimalPlaces", 2);
    }

    /// <summary>
    /// Calculates tax on the subtotal after applying discount.
    /// Formula: (subtotal - discount) × taxRate
    /// </summary>
    public decimal CalculateTax(decimal subtotal, decimal discount = 0)
    {
        var taxableAmount = subtotal - discount;
        var tax = taxableAmount * _taxRate;
        return Math.Round(tax, _roundToDecimalPlaces);
    }

    /// <summary>
    /// Gets the current tax rate from configuration
    /// </summary>
    public decimal GetTaxRate()
    {
        return _taxRate;
    }
}
