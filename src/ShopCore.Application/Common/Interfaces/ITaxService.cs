namespace ShopCore.Application.Common.Interfaces;

/// <summary>
/// Service for calculating tax amounts
/// </summary>
public interface ITaxService
{
    /// <summary>
    /// Calculates tax on a given subtotal after applying discount
    /// </summary>
    /// <param name="subtotal">The subtotal amount before tax and discount</param>
    /// <param name="discount">The discount amount to subtract before calculating tax (default: 0)</param>
    /// <returns>The calculated tax amount, rounded to 2 decimal places</returns>
    decimal CalculateTax(decimal subtotal, decimal discount = 0);

    /// <summary>
    /// Gets the current tax rate (e.g., 0.18 for 18% GST)
    /// </summary>
    /// <returns>The tax rate as a decimal</returns>
    decimal GetTaxRate();
}
