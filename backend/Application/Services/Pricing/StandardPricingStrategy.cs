using BikeRentalApp.Application.Services.Interfaces;

namespace BikeRentalApp.Application.Services.Pricing;

public class StandardPricingStrategy : IPricingStrategy
{
    public string Name => "Standard";

    public decimal CalculatePrice(decimal basePrice, double durationMinutes)
    {
        return (decimal)Math.Ceiling(durationMinutes) * basePrice;
    }
}
