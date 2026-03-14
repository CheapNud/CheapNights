namespace CheapNights.Services;

public class NowPlayingService
{
    public event Func<Task>? OnChange;

    public async Task NotifyChangedAsync()
    {
        if (OnChange is null) return;

        foreach (var handler in OnChange.GetInvocationList().Cast<Func<Task>>())
        {
            try
            {
                await handler();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"NowPlayingService handler failed: {ex.Message}");
            }
        }
    }
}
