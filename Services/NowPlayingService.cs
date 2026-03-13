namespace CheapNights.Services;

public class NowPlayingService
{
    public event Action? OnChange;
    public void NotifyChanged() => OnChange?.Invoke();
}
