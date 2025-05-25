namespace BikeRentalApp.Domain.Entities;

public class Rental
{
    protected Rental() { }

    public Rental(
        Guid id,
        Guid userId,
        Guid bikeId,
        Guid startZoneId,
        DateTime startTime,
        Guid reservationId
    )
    {
        Id = id;
        UserId = userId;
        BikeId = bikeId;
        StartZoneId = startZoneId;
        StartTime = startTime;
        ReservationId = reservationId;
    }

    public Guid Id { get; init; }

    public Guid BikeId { get; private set; }
    public Bike Bike { get; private set; } = null!;

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }

    public Guid StartZoneId { get; private set; }
    public Zone StartZone { get; private set; } = null!;

    public Guid? EndZoneId { get; private set; }
    public Zone? EndZone { get; private set; }

    public Payment? Payment { get; private set; }

    public Guid ReservationId { get; private set; }
    public Reservation Reservation { get; private set; } = null!;

    public void End(Guid endZoneId)
    {
        EndTime = DateTime.UtcNow;
        EndZoneId = endZoneId;
    }
}
