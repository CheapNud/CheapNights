namespace CheapNights.Services;

public class NowPlayingService
{
    public event Func<Task>? OnChange;

    public async Task NotifyChangedAsync()
    {
        if (OnChange is not null)
            await OnChange.Invoke();
    }
}
