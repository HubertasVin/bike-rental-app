namespace BikeRentalApp.Application.DTOs;

public class UpdateUserDTO
{
    public required string Name {get; set;}

    public required string Email {get; set;}

    public string? Password {get; set;}

}