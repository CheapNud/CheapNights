using CheapHelpers.EF.Repositories;
using CheapNights.Data;
using CheapNights.Models;
using Microsoft.EntityFrameworkCore;

namespace CheapNights.Repositories;

public class StatusRepo(IDbContextFactory<HorrorDbContext> factory) : BaseRepo<HorrorDbContext>(factory)
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
