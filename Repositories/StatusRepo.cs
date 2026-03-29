using CheapHelpers.EF.Repositories;
using CheapNights.Data;
using CheapNights.Models;
using Microsoft.EntityFrameworkCore;

namespace CheapNights.Repositories;

public class StatusRepo(IDbContextFactory<CheapNightsDbContext> factory) : BaseRepo<CheapNightsDbContext>(factory)
{
    public async Task<List<Status>> GetSelectableAsync()
    {
        using var db = _factory.CreateDbContext();
        return await db.Statuses
            .Where(s => s.IsSelectable)
            .OrderBy(s => s.SortOrder)
            .AsNoTracking()
            .ToListAsync();
    }
}
