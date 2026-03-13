using CheapNights.Data;
using CheapNights.Models;
using Microsoft.EntityFrameworkCore;

namespace CheapNights.Services;

public class SessionService(IDbContextFactory<HorrorDbContext> factory)
{
    public async Task<List<PlannedSession>> GetSessionsForMonthAsync(int year, int month)
    {
        await using var db = await factory.CreateDbContextAsync();
        var startOfMonth = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var startOfNext = startOfMonth.AddMonths(1);

        return await db.PlannedSessions
            .Include(s => s.GameEntry)
            .Where(s => s.ScheduledAt >= startOfMonth && s.ScheduledAt < startOfNext)
            .OrderBy(s => s.ScheduledAt)
            .ToListAsync();
    }

    public async Task<List<PlannedSession>> GetUpcomingAsync(int count = 8)
    {
        await using var db = await factory.CreateDbContextAsync();
        var now = DateTime.UtcNow.Date;

        return await db.PlannedSessions
            .Include(s => s.GameEntry)
            .Where(s => s.ScheduledAt >= now)
            .OrderBy(s => s.ScheduledAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<PlannedSession?> GetByDateAsync(DateTime date)
    {
        await using var db = await factory.CreateDbContextAsync();
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        return await db.PlannedSessions
            .Include(s => s.GameEntry)
            .FirstOrDefaultAsync(s => s.ScheduledAt >= dayStart && s.ScheduledAt < dayEnd);
    }

    public async Task SaveSessionAsync(PlannedSession session)
    {
        await using var db = await factory.CreateDbContextAsync();
        if (session.Id == 0)
            db.PlannedSessions.Add(session);
        else
            db.PlannedSessions.Update(session);

        await db.SaveChangesAsync();
    }

    public async Task DeleteSessionAsync(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        var session = await db.PlannedSessions.FindAsync(id);
        if (session is null) return;
        db.PlannedSessions.Remove(session);
        await db.SaveChangesAsync();
    }
}
