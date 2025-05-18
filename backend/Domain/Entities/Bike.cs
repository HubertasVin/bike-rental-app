using BikeRentalApp.Domain.Enums;

namespace BikeRentalApp.Domain.Entities;

public class Bike
{
    public Bike(
        Guid id,
        decimal pricePerMinute,
        string model,
        BikeStatus status,
        LockStatus lockStatus,
        Guid zoneId
    )
    {
        Id = id;
        PricePerMinute = pricePerMinute;
        Model = model;
        Status = status;
        LockStatus = lockStatus;
        ZoneId = zoneId;
    }

    public Guid Id { get; init; }
    public decimal PricePerMinute { get; private set; }
    public string Model { get; private set; } = null!;
    public BikeStatus Status { get; private set; }
    public LockStatus LockStatus { get; private set; }

    public Guid ZoneId { get; private set; }
    public Zone Zone { get; private set; } = null!;

    public ICollection<Rental> Rentals { get; private set; } = new List<Rental>();
    public ICollection<Reservation> Reservations { get; private set; } = new List<Reservation>();
    public ICollection<Report> Reports { get; private set; } = new List<Report>();

    public void MarkAsRented() => Status = BikeStatus.Rented;

    public void MarkAsReserved() => Status = BikeStatus.Reserved;

    public void MarkAsAvailable() => Status = BikeStatus.Available;

    public void Lock() => LockStatus = LockStatus.Locked;

    public void Unlock() => LockStatus = LockStatus.Unlocked;
}
