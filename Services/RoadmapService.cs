using CheapNights.Models;
using CheapNights.Repositories;

namespace CheapNights.Services;

public class RoadmapService(GameEntryRepo repo)
{
    public Task<List<GameEntry>> GetAllAsync() => repo.GetAllOrderedAsync();

    public Task<List<GameEntry>> GetByCategoryAsync(int categoryId) => repo.GetByCategoryAsync(categoryId);

    public Task MarkCompletedAsync(int id, string? completedNote) => repo.MarkCompletedAsync(id, completedNote);

    public Task UnmarkCompletedAsync(int id) => repo.UnmarkCompletedAsync(id);

    public Task SaveEntryAsync(GameEntry updated) => repo.SaveEntryAsync(updated);
}
