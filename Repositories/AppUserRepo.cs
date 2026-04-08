using CheapHelpers.EF.Repositories;
using CheapNights.Data;
using CheapNights.Models;
using Microsoft.EntityFrameworkCore;

namespace CheapNights.Repositories;

public class AppUserRepo(IDbContextFactory<CheapNightsDbContext> factory) : BaseRepo<CheapNightsDbContext>(factory)
{
    public async Task<AppUser?> GetByPlexIdAsync(string plexUserId)
    {
        using var db = _factory.CreateDbContext();
        return await db.AppUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.PlexUserId == plexUserId);
    }

    public async Task<List<AppUser>> GetUsersNotInGroupAsync(int groupId)
    {
        using var db = _factory.CreateDbContext();
        var memberUserIds = db.GroupMembers
            .Where(m => m.GroupId == groupId)
            .Select(m => m.AppUserId);

        return await db.AppUsers
            .Where(u => !memberUserIds.Contains(u.Id))
            .OrderBy(u => u.DisplayName)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<AppUser?> GetByCalendarTokenAsync(Guid token)
    {
        using var db = _factory.CreateDbContext();
        return await db.AppUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.CalendarToken == token);
    }

    public async Task<Guid?> GetCalendarTokenAsync(int appUserId)
    {
        using var db = _factory.CreateDbContext();
        return await db.AppUsers
            .Where(u => u.Id == appUserId)
            .Select(u => u.CalendarToken)
            .FirstOrDefaultAsync();
    }

    public async Task<Guid> GetOrCreateCalendarTokenAsync(int appUserId)
    {
        using var db = _factory.CreateDbContext();
        var appUser = await db.AppUsers.SingleOrDefaultAsync(u => u.Id == appUserId)
            ?? throw new InvalidOperationException($"AppUser {appUserId} not found");

        if (appUser.CalendarToken.HasValue)
            return appUser.CalendarToken.Value;

        appUser.CalendarToken = Guid.NewGuid();
        await db.SaveChangesAsync();
        return appUser.CalendarToken.Value;
    }

    public async Task<AppUser> GetOrCreateAsync(string plexUserId, string displayName, string? avatarUrl)
    {
        using var db = _factory.CreateDbContext();
        var existing = await db.AppUsers.FirstOrDefaultAsync(u => u.PlexUserId == plexUserId);

        if (existing is not null)
        {
            if (existing.DisplayName != displayName || existing.AvatarUrl != avatarUrl)
            {
                existing.DisplayName = displayName;
                existing.AvatarUrl = avatarUrl;
                await db.SaveChangesAsync();
            }
            return existing;
        }

        var appUser = new AppUser
        {
            PlexUserId = plexUserId,
            DisplayName = displayName,
            AvatarUrl = avatarUrl,
            CreatedAt = DateTime.UtcNow
        };
        db.AppUsers.Add(appUser);
        await db.SaveChangesAsync();
        return appUser;
    }
}
