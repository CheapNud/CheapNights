using CheapHelpers.EF.Repositories;
using CheapNights.Data;
using CheapNights.Models;
using Microsoft.EntityFrameworkCore;

namespace CheapNights.Repositories;

public class SessionRepo(IDbContextFactory<HorrorDbContext> factory) : BaseRepo<HorrorDbContext>(factory)
{
    public async Task<List<PlannedSession>> GetForMonthAsync(int year, int month)
    {
        using var db = _factory.CreateDbContext();
        var startOfMonth = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var startOfNext = startOfMonth.AddMonths(1);

        return await db.PlannedSessions
            .Include(s => s.GameEntry)
            .Where(s => s.ScheduledAt >= startOfMonth && s.ScheduledAt < startOfNext)
            .OrderBy(s => s.ScheduledAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<PlannedSession>> GetUpcomingAsync(int count)
    {
        using var db = _factory.CreateDbContext();
        var now = DateTime.UtcNow.Date.ToUniversalTime();

        return await db.PlannedSessions
            .Include(s => s.GameEntry)
            .Where(s => s.ScheduledAt >= now)
            .OrderBy(s => s.ScheduledAt)
            .Take(count)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<PlannedSession?> GetByDateAsync(DateTime date)
    {
        using var db = _factory.CreateDbContext();
        var dayStart = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var dayEnd = dayStart.AddDays(1);

        return await db.PlannedSessions
            .Include(s => s.GameEntry)
            .FirstOrDefaultAsync(s => s.ScheduledAt >= dayStart && s.ScheduledAt < dayEnd);
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
        var session = await db.PlannedSessions.FindAsync(id);
        if (session is null) return;

        db.PlannedSessions.Remove(session);
        await db.SaveChangesAsync();
    }
}
