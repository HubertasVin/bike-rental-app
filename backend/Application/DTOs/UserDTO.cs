namespace BikeRentalApp.Application.DTOs;

public class UserDTO
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }

    public required string Role { get; set; }
}
