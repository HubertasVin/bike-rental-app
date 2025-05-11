using System;

namespace BikeRentalApp.Application.DTOs;

public class CreateReportDTO
{
    public Guid BikeId { get; set; }
    public Guid? UserId { get; set; }
    public string Type { get; set; } = null!;
    public string Description { get; set; } = null!;
}
