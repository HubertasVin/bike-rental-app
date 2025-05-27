using BikeRentalApp.Application.Services.Interfaces;

namespace BikeRentalApp.Application.Services.Pricing;

public class DiscountedPricingStrategy : IPricingStrategy
{
    public string Name => "Discounted";

    public decimal CalculatePrice(decimal basePrice, double durationMinutes)
    {
        var standardPrice = (decimal)Math.Ceiling(durationMinutes) * basePrice;
        // 50% discount
        return standardPrice * 0.5m;
    }
}
