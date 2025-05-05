namespace BikeRentalApp.Application.DTOs;

public class ZoneDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public double Latitude1 { get; set; }
    public double Longitude1 { get; set; }
    public double Latitude2 { get; set; }
    public double Longitude2 { get; set; }
    public int Capacity { get; set; }

    public ICollection<BikeDTO> Bikes { get; set; } = new List<BikeDTO>();
}
