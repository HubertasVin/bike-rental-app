using BikeRentalApp.Domain.Enums;

namespace BikeRentalApp.Domain.Entities;

public class User
{
    protected User() { }

    public User(Guid id, string name, string email, string plainPassword)
    {
        Id = id;
        Name = name;
        Email = email;
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        Roles.Add(Role.User);
    }

    public Guid Id { get; init; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public ICollection<Role> Roles { get; private set; } = new List<Role>();

    public ICollection<Rental> Rentals { get; private set; } = new List<Rental>();
    public ICollection<Reservation> Reservations { get; private set; } = new List<Reservation>();
    public ICollection<Payment> Payments { get; private set; } = new List<Payment>();
    public ICollection<Report> Reports { get; private set; } = new List<Report>();
    public ICollection<Authentication> Authentications { get; private set; } =
        new List<Authentication>();

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void UpdateEmail(string email)
    {
        Email = email;
    }

    public void UpdatePassword(string plainPassword)
    {
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
    }
}
