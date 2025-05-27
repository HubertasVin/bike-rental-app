public interface ILoggingToggleService
{
    bool Enabled { get; }
    void SetEnabled(bool enabled);
}
