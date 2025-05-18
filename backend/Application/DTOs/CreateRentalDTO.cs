namespace BikeRentalApp.Application.DTOs;

public class CreateRentalDTO
{
    public Guid BikeId { get; set; }
    public Guid StartZoneId { get; set; }
}
