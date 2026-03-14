using CheapHelpers.EF.Repositories;
using CheapNights.Data;
using CheapNights.Models;
using Microsoft.EntityFrameworkCore;

namespace CheapNights.Repositories;

public class GameEntryRepo(IDbContextFactory<HorrorDbContext> factory) : BaseRepo<HorrorDbContext>(factory)
{
    public async Task<List<GameEntry>> GetByCategoryAsync(string category)
    {
        using var db = _factory.CreateDbContext();
        return await db.GameEntries
            .Include(g => g.Status)
            .Where(g => g.Category == category)
            .OrderBy(g => g.SortOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<GameEntry>> GetSelectableGamesAsync()
    {
        using var db = _factory.CreateDbContext();
        return await db.GameEntries
            .Include(g => g.Status)
            .Where(g => g.Status == null || g.Status.IsSelectable)
            .OrderBy(g => g.SortOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<GameEntry>> GetAllOrderedAsync()
    {
        using var db = _factory.CreateDbContext();
        return await db.GameEntries
            .Include(g => g.Status)
            .OrderBy(g => g.SortOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task SaveEntryAsync(GameEntry updated)
    {
        using var db = _factory.CreateDbContext();
        var entry = await db.GameEntries.FindAsync(updated.Id);
        if (entry is null) return;

        entry.Name = updated.Name;
        entry.Category = updated.Category;
        entry.EntryType = updated.EntryType;
        entry.IsMovie = updated.IsMovie;
        entry.StatusId = updated.StatusId;
        entry.GameNote = updated.GameNote;
        entry.Protagonist = updated.Protagonist;
        entry.StoryEra = updated.StoryEra;
        entry.StarRating = updated.StarRating;
        entry.LengthLabel = updated.LengthLabel;
        entry.PlatformBrecht = updated.PlatformBrecht;
        entry.PlatformPieter = updated.PlatformPieter;
        entry.IsCouchCoop = updated.IsCouchCoop;
        entry.SortLabel = updated.SortLabel;
        entry.SortOrder = updated.SortOrder;
        entry.IsCompleted = updated.IsCompleted;
        entry.CompletedAt = updated.CompletedAt;
        entry.CompletedNote = updated.CompletedNote;

        await db.SaveChangesAsync();
    }

    public async Task MarkCompletedAsync(int id, string? completedNote)
    {
        using var db = _factory.CreateDbContext();
        var entry = await db.GameEntries.FindAsync(id);
        if (entry is null) return;

        entry.IsCompleted = true;
        entry.CompletedAt = DateTime.UtcNow;
        entry.CompletedNote = completedNote;
        await db.SaveChangesAsync();
    }

    public async Task UnmarkCompletedAsync(int id)
    {
        using var db = _factory.CreateDbContext();
        var entry = await db.GameEntries.FindAsync(id);
        if (entry is null) return;

        entry.IsCompleted = false;
        entry.CompletedAt = null;
        entry.CompletedNote = null;
        await db.SaveChangesAsync();
    }
}
