using System;

namespace BikeRentalApp.Application.DTOs;

public class CreateBikeDTO
{
    public decimal PricePerMinute { get; set; }
    public string Model { get; set; } = null!;
    public Guid ZoneId { get; set; }
}
