using System;

namespace BikeRentalApp.Application.DTOs;

public class BikeLocationDTO
{
    public Guid BikeId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Guid ZoneId { get; set; }
}
