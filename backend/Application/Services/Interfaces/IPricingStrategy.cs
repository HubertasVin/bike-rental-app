namespace BikeRentalApp.Application.Services.Interfaces;

public interface IPricingStrategy
{
    decimal CalculatePrice(decimal basePrice, double durationMinutes);
    string Name { get; }
}
