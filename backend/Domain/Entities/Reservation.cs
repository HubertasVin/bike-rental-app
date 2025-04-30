public class Reservation
{
    public Guid Id { get; init; }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Guid BikeId { get; private set; }
    public Bike Bike { get; private set; } = null!;

    public DateTime StartTime { get; private set; }
    public DateTime ExpirationTime { get; private set; }
}
