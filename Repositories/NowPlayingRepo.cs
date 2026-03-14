using CheapHelpers.EF.Repositories;
using CheapNights.Constants;
using CheapNights.Data;
using CheapNights.Models;
using Microsoft.EntityFrameworkCore;

namespace CheapNights.Repositories;

public class NowPlayingRepo(IDbContextFactory<HorrorDbContext> factory) : BaseRepo<HorrorDbContext>(factory)
{
    public async Task<NowPlaying?> GetCurrentAsync()
    {
        using var db = _factory.CreateDbContext();
        return await db.NowPlaying
            .Include(n => n.GameEntry)
            .FirstOrDefaultAsync(n => n.Id == GameConstants.Defaults.NowPlayingId);
    }

    public async Task SetCurrentAsync(int? gameEntryId, string? statusNote)
    {
        using var db = _factory.CreateDbContext();
        var np = await db.NowPlaying.FirstOrDefaultAsync(n => n.Id == GameConstants.Defaults.NowPlayingId);
        if (np is null) return;

        np.GameEntryId = gameEntryId;
        np.StatusNote = statusNote;
        np.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task ClearAsync()
    {
        await SetCurrentAsync(null, null);
    }
}
