public class Rental
{
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
}
