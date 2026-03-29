using CheapNights.Models;
using CheapNights.Repositories;

namespace CheapNights.Services;

public class SessionService(SessionRepo repo)
{
    public Task<List<PlannedSession>> GetSessionsForMonthAsync(int groupId, int year, int month) =>
        repo.GetForMonthAsync(groupId, year, month);

    public Task<List<PlannedSession>> GetUpcomingAsync(int groupId, int count = 8) =>
        repo.GetUpcomingAsync(groupId, count);

    public Task<PlannedSession?> GetByDateAsync(int groupId, DateTime date) =>
        repo.GetByDateAsync(groupId, date);

    public Task SaveSessionAsync(PlannedSession session) => repo.SaveSessionAsync(session);

    public Task DeleteSessionAsync(int id) => repo.DeleteSessionAsync(id);
}
