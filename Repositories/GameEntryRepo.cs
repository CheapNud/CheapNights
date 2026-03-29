using CheapHelpers.EF.Repositories;
using CheapNights.Data;
using CheapNights.Models;
using Microsoft.EntityFrameworkCore;

namespace CheapNights.Repositories;

public class GameEntryRepo(IDbContextFactory<CheapNightsDbContext> factory) : BaseRepo<CheapNightsDbContext>(factory)
{
    public async Task<List<GameEntry>> GetByCategoryAsync(int categoryId, int groupId)
    {
        using var db = _factory.CreateDbContext();
        return await db.GameEntries
            .Include(g => g.Status)
            .Include(g => g.Category)
            .Include(g => g.EntryType)
            .Include(g => g.MemberPlatforms)
                .ThenInclude(mp => mp.Platform)
            .Include(g => g.MemberPlatforms)
                .ThenInclude(mp => mp.GroupMember)
                    .ThenInclude(gm => gm!.AppUser)
            .Where(g => g.CategoryId == categoryId && g.GroupId == groupId)
            .OrderBy(g => g.SortOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<GameEntry>> GetSelectableGamesAsync(int groupId)
    {
        using var db = _factory.CreateDbContext();
        return await db.GameEntries
            .Include(g => g.Status)
            .Where(g => g.GroupId == groupId && (g.Status == null || g.Status.IsSelectable))
            .OrderBy(g => g.SortOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<GameEntry>> GetAllOrderedAsync(int groupId)
    {
        using var db = _factory.CreateDbContext();
        return await db.GameEntries
            .Include(g => g.Status)
            .Include(g => g.Category)
            .Include(g => g.EntryType)
            .Include(g => g.MemberPlatforms)
                .ThenInclude(mp => mp.Platform)
            .Include(g => g.MemberPlatforms)
                .ThenInclude(mp => mp.GroupMember)
                    .ThenInclude(gm => gm!.AppUser)
            .Where(g => g.GroupId == groupId)
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
        entry.IsCouchCoop = updated.IsCouchCoop;
        entry.SortLabel = updated.SortLabel;
        entry.SortOrder = updated.SortOrder;
        entry.IsCompleted = updated.IsCompleted;
        entry.CompletedAt = updated.CompletedAt;
        entry.CompletedTime = updated.CompletedTime;

        await db.SaveChangesAsync();
    }

    public async Task SaveMemberPlatformAsync(int gameEntryId, int groupMemberId, int? platformId)
    {
        using var db = _factory.CreateDbContext();
        var existing = await db.MemberGamePlatforms
            .FirstOrDefaultAsync(mp => mp.GameEntryId == gameEntryId && mp.GroupMemberId == groupMemberId);

        if (platformId is null)
        {
            if (existing is not null)
            {
                db.MemberGamePlatforms.Remove(existing);
                await db.SaveChangesAsync();
            }
            return;
        }

        if (existing is not null)
        {
            existing.PlatformId = platformId.Value;
        }
        else
        {
            db.MemberGamePlatforms.Add(new MemberGamePlatform
            {
                GroupMemberId = groupMemberId,
                GameEntryId = gameEntryId,
                PlatformId = platformId.Value
            });
        }
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
