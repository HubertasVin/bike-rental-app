using System;

namespace BikeRentalApp.Application.DTOs;

public class CreateBikeDTO
{
    public decimal RentPrice { get; set; }
    public string Model { get; set; } = null!;
    public Guid ZoneId { get; set; }
}
