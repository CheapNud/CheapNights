using CheapHelpers.EF.Repositories;
using CheapNights.Data;
using CheapNights.Models;
using Microsoft.EntityFrameworkCore;

namespace CheapNights.Repositories;

public class GameEntryRepo(IDbContextFactory<HorrorDbContext> factory) : BaseRepo<HorrorDbContext>(factory)
{
    public async Task<List<GameEntry>> GetByCategoryAsync(int categoryId)
    {
        using var db = _factory.CreateDbContext();
        return await db.GameEntries
            .Include(g => g.Status)
            .Include(g => g.Category)
            .Include(g => g.EntryType)
            .Include(g => g.PlatformBrecht)
            .Include(g => g.PlatformPieter)
            .Where(g => g.CategoryId == categoryId)
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
            .Include(g => g.Category)
            .Include(g => g.EntryType)
            .Include(g => g.PlatformBrecht)
            .Include(g => g.PlatformPieter)
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
        entry.CategoryId = updated.CategoryId;
        entry.EntryTypeId = updated.EntryTypeId;
        entry.IsMovie = updated.IsMovie;
        entry.StatusId = updated.StatusId;
        entry.GameNote = updated.GameNote;
        entry.Protagonist = updated.Protagonist;
        entry.StoryEra = updated.StoryEra;
        entry.StarRating = updated.StarRating;
        entry.LengthLabel = updated.LengthLabel;
        entry.PlatformBrechtId = updated.PlatformBrechtId;
        entry.PlatformPieterId = updated.PlatformPieterId;
        entry.IsCouchCoop = updated.IsCouchCoop;
        entry.SortLabel = updated.SortLabel;
        entry.SortOrder = updated.SortOrder;
        entry.IsCompleted = updated.IsCompleted;
        entry.CompletedAt = updated.CompletedAt;
        entry.CompletedTime = updated.CompletedTime;

        await db.SaveChangesAsync();
    }

    public async Task MarkCompletedAsync(int id, string? completedTime)
    {
        using var db = _factory.CreateDbContext();
        var entry = await db.GameEntries.FindAsync(id);
        if (entry is null) return;

        entry.IsCompleted = true;
        entry.CompletedAt = DateTime.UtcNow;
        entry.CompletedTime = completedTime;
        await db.SaveChangesAsync();
    }

    public async Task UnmarkCompletedAsync(int id)
    {
        using var db = _factory.CreateDbContext();
        var entry = await db.GameEntries.FindAsync(id);
        if (entry is null) return;

        entry.IsCompleted = false;
        entry.CompletedAt = null;
        entry.CompletedTime = null;
        await db.SaveChangesAsync();
    }
}
