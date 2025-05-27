public class LoggingToggleService : ILoggingToggleService
{
    private volatile bool _enabled = true;
    public bool Enabled => _enabled;

    public void SetEnabled(bool enabled) => _enabled = enabled;
}
