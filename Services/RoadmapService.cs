using CheapNights.Data;
using CheapNights.Models;
using Microsoft.EntityFrameworkCore;

namespace CheapNights.Services;

public class RoadmapService(IDbContextFactory<HorrorDbContext> factory)
{
    public async Task<List<GameEntry>> GetAllAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.GameEntries.OrderBy(g => g.SortOrder).ToListAsync();
    }

    public async Task<List<GameEntry>> GetByCategoryAsync(string category)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.GameEntries
            .Where(g => g.Category == category)
            .OrderBy(g => g.SortOrder)
            .ToListAsync();
    }

    public async Task MarkCompletedAsync(int id, string? completedNote)
    {
        await using var db = await factory.CreateDbContextAsync();
        var entry = await db.GameEntries.FindAsync(id);
        if (entry is null) return;
        entry.IsCompleted = true;
        entry.CompletedAt = DateTime.UtcNow;
        entry.CompletedNote = completedNote;
        await db.SaveChangesAsync();
    }

    public async Task UnmarkCompletedAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        var entry = await db.GameEntries.FindAsync(id);
        if (entry is null) return;
        entry.IsCompleted = false;
        entry.CompletedAt = null;
        entry.CompletedNote = null;
        await db.SaveChangesAsync();
    }

    public async Task SaveEntryAsync(GameEntry updated)
    {
        await using var db = await factory.CreateDbContextAsync();
        var entry = await db.GameEntries.FindAsync(updated.Id);
        if (entry is null) return;

        entry.Name = updated.Name;
        entry.Category = updated.Category;
        entry.EntryType = updated.EntryType;
        entry.IsMovie = updated.IsMovie;
        entry.StatusLabel = updated.StatusLabel;
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
}
