public class Zone
{
    public Guid Id { get; init; }
    public string Name { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public int Capacity { get; private set; }

    public ICollection<Bike> Bikes { get; private set; } = new List<Bike>();
}
