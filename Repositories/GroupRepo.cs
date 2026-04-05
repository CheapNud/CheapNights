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
        var groupIds = await db.GroupMembers
            .Where(m => m.AppUserId == appUserId)
            .Select(m => m.GroupId)
            .ToListAsync();

        return await db.Groups
            .Include(g => g.Owner)
            .Where(g => groupIds.Contains(g.Id))
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

    public async Task CreateGroupAsync(Group group, int callerUserId)
    {
        if (group.OwnerId != callerUserId)
            throw new UnauthorizedAccessException("Group owner must match the authenticated user");

        using var db = _factory.CreateDbContext();

        db.Groups.Add(group);
        db.GroupMembers.Add(new GroupMember
        {
            Group = group,
            AppUserId = callerUserId
        });

        await db.SaveChangesAsync();
    }

    public async Task UpdateGroupAsync(Group group, int callerUserId)
    {
        using var db = _factory.CreateDbContext();
        var existing = await db.Groups.SingleOrDefaultAsync(g => g.Id == group.Id)
            ?? throw new InvalidOperationException($"Group {group.Id} not found");

        if (existing.OwnerId != callerUserId)
            throw new UnauthorizedAccessException($"User {callerUserId} is not the owner of group {group.Id}");

        existing.Name = group.Name;
        existing.Description = group.Description;
        existing.ThemeColor = group.ThemeColor;
        existing.ThemePreset = group.ThemePreset;
        existing.IconName = group.IconName;

        await db.SaveChangesAsync();
    }

    public async Task AddMemberAsync(GroupMember member, int callerUserId)
    {
        using var db = _factory.CreateDbContext();
        var group = await db.Groups.SingleOrDefaultAsync(g => g.Id == member.GroupId)
            ?? throw new InvalidOperationException($"Group {member.GroupId} not found");

        if (group.OwnerId != callerUserId)
            throw new UnauthorizedAccessException($"User {callerUserId} is not the owner of group {member.GroupId}");

        db.GroupMembers.Add(member);
        await db.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(int groupMemberId, int callerUserId)
    {
        using var db = _factory.CreateDbContext();
        var member = await db.GroupMembers
            .Include(m => m.Group)
            .SingleOrDefaultAsync(m => m.Id == groupMemberId);
        if (member is null) return;

        if (member.Group!.OwnerId != callerUserId)
            throw new UnauthorizedAccessException($"User {callerUserId} is not the owner of group {member.GroupId}");

        if (member.AppUserId == member.Group.OwnerId)
            throw new InvalidOperationException("Cannot remove the group owner as a member");

        await db.PlannedSessions
            .Where(s => s.HostMemberId == groupMemberId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.HostMemberId, (int?)null));

        await db.MemberGamePlatforms
            .Where(p => p.GroupMemberId == groupMemberId)
            .ExecuteDeleteAsync();

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
