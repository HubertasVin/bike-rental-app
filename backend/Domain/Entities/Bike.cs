public class Bike
{
    public Guid Id { get; init; }
    public decimal RentPrice { get; private set; }
    public string Model { get; private set; } = null!;
    public BikeStatus Status { get; private set; }
    public LockStatus LockStatus { get; private set; }

    public Guid ZoneId { get; private set; }
    public Zone Zone { get; private set; } = null!;

    public ICollection<Rental> Rentals { get; private set; } = new List<Rental>();
    public ICollection<Reservation> Reservations { get; private set; } = new List<Reservation>();
    public ICollection<Report> Reports { get; private set; } = new List<Report>();

    public void MarkAsRented() => Status = BikeStatus.Rented;
}
