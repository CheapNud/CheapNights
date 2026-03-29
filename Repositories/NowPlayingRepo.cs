using CheapHelpers.EF.Repositories;
using CheapNights.Data;
using CheapNights.Models;
using Microsoft.EntityFrameworkCore;

namespace CheapNights.Repositories;

public class NowPlayingRepo(IDbContextFactory<CheapNightsDbContext> factory) : BaseRepo<CheapNightsDbContext>(factory)
{
    public async Task<NowPlaying?> GetCurrentAsync(int groupId)
    {
        using var db = _factory.CreateDbContext();
        return await db.NowPlaying
            .Include(n => n.GameEntry)
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.GroupId == groupId);
    }

    /// <summary>
    /// Sets the currently playing game for a group. When switching to a different game,
    /// the previous game is automatically marked as completed (IsCompleted + CompletedAt).
    /// </summary>
    public async Task SetCurrentAsync(int groupId, int? gameEntryId, string? statusNote)
    {
        using var db = _factory.CreateDbContext();
        var np = await db.NowPlaying.FirstOrDefaultAsync(n => n.GroupId == groupId);

        if (np is null)
        {
            np = new NowPlaying { GroupId = groupId };
            db.NowPlaying.Add(np);
        }

        if (np.GameEntryId is not null && np.GameEntryId != gameEntryId)
        {
            var previousGame = await db.GameEntries.FindAsync(np.GameEntryId);
            if (previousGame is not null && !previousGame.IsCompleted)
            {
                previousGame.IsCompleted = true;
                previousGame.CompletedAt = DateTime.UtcNow;
            }
        }

        np.GameEntryId = gameEntryId;
        np.StatusNote = statusNote;
        np.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task ClearAsync(int groupId)
    {
        await SetCurrentAsync(groupId, null, null);
    }
}
