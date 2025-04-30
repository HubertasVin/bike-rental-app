public class Authentication
{
    public Guid Id { get; init; }
    public string Token { get; private set; } = null!;
    public DateTime ExpirationTime { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
}
