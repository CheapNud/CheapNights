using CheapHelpers.EF.Repositories;
using CheapNights.Data;
using CheapNights.Models;
using Microsoft.EntityFrameworkCore;

namespace CheapNights.Repositories;

public class SessionRepo(IDbContextFactory<CheapNightsDbContext> factory) : BaseRepo<CheapNightsDbContext>(factory)
{
    public async Task<List<PlannedSession>> GetForMonthAsync(int groupId, int year, int month)
    {
        using var db = _factory.CreateDbContext();
        var startOfMonth = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var startOfNext = startOfMonth.AddMonths(1);

        return await db.PlannedSessions
            .Include(s => s.GameEntry)
            .Include(s => s.HostMember)
                .ThenInclude(m => m!.AppUser)
            .Where(s => s.GroupId == groupId && s.ScheduledAt >= startOfMonth && s.ScheduledAt < startOfNext)
            .OrderBy(s => s.ScheduledAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<PlannedSession>> GetUpcomingAsync(int groupId, int count)
    {
        using var db = _factory.CreateDbContext();
        var now = DateTime.UtcNow.Date.ToUniversalTime();

        return await db.PlannedSessions
            .Include(s => s.GameEntry)
            .Include(s => s.HostMember)
                .ThenInclude(m => m!.AppUser)
            .Where(s => s.GroupId == groupId && s.ScheduledAt >= now)
            .OrderBy(s => s.ScheduledAt)
            .Take(count)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<PlannedSession?> GetByDateAsync(int groupId, DateTime date)
    {
        using var db = _factory.CreateDbContext();
        var dayStart = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var dayEnd = dayStart.AddDays(1);

        return await db.PlannedSessions
            .Include(s => s.GameEntry)
            .Include(s => s.HostMember)
                .ThenInclude(m => m!.AppUser)
            .FirstOrDefaultAsync(s => s.GroupId == groupId && s.ScheduledAt >= dayStart && s.ScheduledAt < dayEnd);
    }

    public async Task<List<PlannedSession>> GetForMonthAllGroupsAsync(IEnumerable<int> groupIds, int year, int month)
    {
        using var db = _factory.CreateDbContext();
        var startOfMonth = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var startOfNext = startOfMonth.AddMonths(1);
        var ids = groupIds.ToList();

        return await db.PlannedSessions
            .Include(s => s.GameEntry)
            .Include(s => s.Group)
            .Include(s => s.HostMember)
                .ThenInclude(m => m!.AppUser)
            .Where(s => ids.Contains(s.GroupId) && s.ScheduledAt >= startOfMonth && s.ScheduledAt < startOfNext)
            .OrderBy(s => s.ScheduledAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<PlannedSession>> GetUpcomingAllGroupsAsync(IEnumerable<int> groupIds, int count)
    {
        using var db = _factory.CreateDbContext();
        var now = DateTime.UtcNow.Date.ToUniversalTime();
        var ids = groupIds.ToList();

        return await db.PlannedSessions
            .Include(s => s.GameEntry)
            .Include(s => s.Group)
            .Include(s => s.HostMember)
                .ThenInclude(m => m!.AppUser)
            .Where(s => ids.Contains(s.GroupId) && s.ScheduledAt >= now)
            .OrderBy(s => s.ScheduledAt)
            .Take(count)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<PlannedSession>> GetByDateAllGroupsAsync(IEnumerable<int> groupIds, DateTime date)
    {
        using var db = _factory.CreateDbContext();
        var dayStart = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var dayEnd = dayStart.AddDays(1);
        var ids = groupIds.ToList();

        return await db.PlannedSessions
            .Include(s => s.GameEntry)
            .Include(s => s.Group)
            .Include(s => s.HostMember)
                .ThenInclude(m => m!.AppUser)
            .Where(s => ids.Contains(s.GroupId) && s.ScheduledAt >= dayStart && s.ScheduledAt < dayEnd)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<PlannedSession>> GetAllForUserAsync(int appUserId)
    {
        using var db = _factory.CreateDbContext();
        var groupIds = await db.GroupMembers
            .Where(m => m.AppUserId == appUserId)
            .Select(m => m.GroupId)
            .ToListAsync();

        return await db.PlannedSessions
            .Include(s => s.GameEntry)
            .Include(s => s.Group)
            .Include(s => s.HostMember)
                .ThenInclude(m => m!.AppUser)
            .Where(s => groupIds.Contains(s.GroupId) && !s.IsCompleted)
            .OrderBy(s => s.ScheduledAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task SaveSessionAsync(PlannedSession session)
    {
        using var db = _factory.CreateDbContext();
        if (session.Id == 0)
            db.PlannedSessions.Add(session);
        else
            db.PlannedSessions.Update(session);

        await db.SaveChangesAsync();
    }

    public async Task DeleteSessionAsync(int id)
    {
        using var db = _factory.CreateDbContext();
        var session = await db.PlannedSessions.SingleOrDefaultAsync(s => s.Id == id);
        if (session is null) return;

        db.PlannedSessions.Remove(session);
        await db.SaveChangesAsync();
    }
}
