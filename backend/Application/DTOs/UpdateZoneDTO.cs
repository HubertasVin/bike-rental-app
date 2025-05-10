namespace BikeRentalApp.Application.DTOs;

public record UpdateZoneDTO(
    string Name,
    string Address,
    double Latitude1,
    double Longitude1,
    double Latitude2,
    double Longitude2,
    int Capacity
);
