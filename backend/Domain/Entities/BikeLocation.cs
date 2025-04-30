public class BikeLocation
{
    public Guid Id { get; init; }

    public Guid BikeId { get; private set; }
    public Bike Bike { get; private set; } = null!;

    public Guid? UserId { get; private set; }
    public User? User { get; private set; }

    public Guid? ZoneId { get; private set; }
    public Zone? Zone { get; private set; }
}
