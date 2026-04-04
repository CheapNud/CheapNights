using CheapHelpers.EF.Repositories;
using CheapNights.Data;
using CheapNights.Models;
using Microsoft.EntityFrameworkCore;

namespace CheapNights.Repositories;

public class GroupRepo(IDbContextFactory<CheapNightsDbContext> factory) : BaseRepo<CheapNightsDbContext>(factory)
{
    public async Task<List<Group>> GetGroupsForUserAsync(int appUserId)
    {
        using var db = _factory.CreateDbContext();
        return await db.GroupMembers
            .Where(m => m.AppUserId == appUserId)
            .Include(m => m.Group)
            .Select(m => m.Group!)
            .OrderBy(g => g.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Group?> GetWithMembersAsync(int groupId)
    {
        using var db = _factory.CreateDbContext();
        return await db.Groups
            .Include(g => g.Members)
                .ThenInclude(m => m.AppUser)
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == groupId);
    }

    public async Task<List<GroupMember>> GetMembersAsync(int groupId)
    {
        using var db = _factory.CreateDbContext();
        return await db.GroupMembers
            .Include(m => m.AppUser)
            .Where(m => m.GroupId == groupId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Category>> GetCategoriesAsync(int groupId)
    {
        using var db = _factory.CreateDbContext();
        return await db.Categories
            .Where(c => c.GroupId == groupId)
            .OrderBy(c => c.SortOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task SaveGroupAsync(Group group)
    {
        using var db = _factory.CreateDbContext();
        if (group.Id == 0)
            db.Groups.Add(group);
        else
            db.Groups.Update(group);

        await db.SaveChangesAsync();
    }

    public async Task AddMemberAsync(GroupMember member)
    {
        using var db = _factory.CreateDbContext();
        db.GroupMembers.Add(member);
        await db.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(int groupMemberId)
    {
        using var db = _factory.CreateDbContext();
        var member = await db.GroupMembers.FindAsync(groupMemberId);
        if (member is null) return;

        db.GroupMembers.Remove(member);
        await db.SaveChangesAsync();
    }

    public async Task SaveCategoryAsync(Category category)
    {
        using var db = _factory.CreateDbContext();
        if (category.Id == 0)
            db.Categories.Add(category);
        else
            db.Categories.Update(category);

        await db.SaveChangesAsync();
    }
}
