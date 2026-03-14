using CheapNights.Models;
using CheapNights.Repositories;

namespace CheapNights.Services;

public class SessionService(SessionRepo repo)
{
    public Task<List<PlannedSession>> GetSessionsForMonthAsync(int year, int month) => repo.GetForMonthAsync(year, month);

    public Task<List<PlannedSession>> GetUpcomingAsync(int count = 8) => repo.GetUpcomingAsync(count);

    public Task<PlannedSession?> GetByDateAsync(DateTime date) => repo.GetByDateAsync(date);

    public Task SaveSessionAsync(PlannedSession session) => repo.SaveSessionAsync(session);

    public Task DeleteSessionAsync(int id) => repo.DeleteSessionAsync(id);
}
