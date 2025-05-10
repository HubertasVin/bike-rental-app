namespace BikeRentalApp.Domain.Entities;

public class Zone
{
    public Zone(
        Guid id,
        string name,
        string address,
        double latitude1,
        double longitude1,
        double latitude2,
        double longitude2,
        int capacity
    )
    {
        Id = id;
        Name = name;
        Address = address;
        Latitude1 = latitude1;
        Longitude1 = longitude1;
        Latitude2 = latitude2;
        Longitude2 = longitude2;
        Capacity = capacity;
        Bikes = new List<Bike>();
    }
    public Guid Id { get; init; }
    public string Name { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public double Latitude1 { get; private set; }
    public double Longitude1 { get; private set; }
    public double Latitude2 { get; private set; }
    public double Longitude2 { get; private set; }
    public int Capacity { get; private set; }

    public ICollection<Bike> Bikes { get; private set; } = new List<Bike>();
}
