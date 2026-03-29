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
