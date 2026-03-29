using CheapNights.Models;
using CheapNights.Repositories;

namespace CheapNights.Services;

public class RoadmapService(GameEntryRepo repo)
{
    public Task<List<GameEntry>> GetAllAsync(int groupId) => repo.GetAllOrderedAsync(groupId);

    public Task<List<GameEntry>> GetByCategoryAsync(int categoryId, int groupId) => repo.GetByCategoryAsync(categoryId, groupId);

    public Task MarkCompletedAsync(int id, string? completedNote) => repo.MarkCompletedAsync(id, completedNote);

    public Task UnmarkCompletedAsync(int id) => repo.UnmarkCompletedAsync(id);

    public Task SaveEntryAsync(GameEntry updated) => repo.SaveEntryAsync(updated);

    public Task SaveMemberPlatformAsync(int gameEntryId, int groupMemberId, int? platformId) =>
        repo.SaveMemberPlatformAsync(gameEntryId, groupMemberId, platformId);
}
