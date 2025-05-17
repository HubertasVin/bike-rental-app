namespace BikeRentalApp.Domain.Entities;

public class Reservation
{
    protected Reservation() { }

    public Reservation(Guid id, Guid userId, Guid bikeId, DateTime startTime)
    {
        Id = id;
        UserId = userId;
        BikeId = bikeId;
        StartTime = startTime;
    }

    public Guid Id { get; init; }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Guid BikeId { get; private set; }
    public Bike Bike { get; private set; } = null!;

    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }

    public void End()
    {
        EndTime = DateTime.UtcNow;
    }
}
