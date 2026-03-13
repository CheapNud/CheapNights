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
}
