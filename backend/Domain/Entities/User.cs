public class User
{
    public Guid Id { get; init; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;

    public ICollection<Rental> Rentals { get; private set; } = new List<Rental>();
    public ICollection<Reservation> Reservations { get; private set; } = new List<Reservation>();
    public ICollection<Payment> Payments { get; private set; } = new List<Payment>();
    public ICollection<Report> Reports { get; private set; } = new List<Report>();
    public ICollection<Authentication> Authentications { get; private set; } = new List<Authentication>();
}
