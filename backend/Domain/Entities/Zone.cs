public class Zone
{
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
