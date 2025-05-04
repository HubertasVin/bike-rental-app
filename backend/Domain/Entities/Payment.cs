using BikeRentalApp.Domain.Enums;

namespace BikeRentalApp.Domain.Entities;

public class Payment
{
    public Guid Id { get; init; }
    public decimal Total { get; private set; }
    public PaymentStatus Status { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Guid RentalId { get; private set; }
    public Rental Rental { get; private set; } = null!;

    public DateTime Date { get; private set; }
}
