namespace BikeRentalApp.Domain.Entities;

public class Report
{
    protected Report() { }

    public Report(
        Guid id,
        Guid bikeId,
        Guid? userId,
        string type,
        string description)
    {
        Id = id;
        BikeId = bikeId;
        UserId = userId;
        Type = type;
        Description = description;
    }

    public Guid Id { get; init; }

    public Guid BikeId { get; private set; }
    public Bike Bike { get; private set; } = null!;

    public Guid? UserId { get; private set; }
    public User? User { get; private set; }

    public string Type { get; private set; } = null!;
    public string Description { get; private set; } = null!;
}