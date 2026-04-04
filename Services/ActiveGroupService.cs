using CheapNights.Models;

namespace CheapNights.Services;

/// <summary>
/// Scoped service holding the user's currently selected group for the circuit.
/// </summary>
public class ActiveGroupService
{
    public Group? ActiveGroup { get; private set; }
    public List<GroupMember> Members { get; private set; } = [];
    public List<Group> UserGroups { get; private set; } = [];
    public bool HasLoaded { get; private set; }
    public int GroupId => ActiveGroup?.Id ?? 0;
    public IEnumerable<int> AllGroupIds => UserGroups.Select(g => g.Id);

    public event Func<Task>? OnGroupChanged;

    public async Task SetActiveGroupAsync(Group group, List<GroupMember> members)
    {
        ActiveGroup = group;
        Members = members;

        if (OnGroupChanged is not null)
        {
            foreach (var handler in OnGroupChanged.GetInvocationList().Cast<Func<Task>>())
            {
                try { await handler(); }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ActiveGroupService handler failed: {ex.Message}");
                }
            }
        }
    }

    public void SetUserGroups(List<Group> groups)
    {
        UserGroups = groups;
        HasLoaded = true;
    }

    public GroupMember? GetMemberByUserId(int appUserId) =>
        Members.FirstOrDefault(m => m.AppUserId == appUserId);

    public string GetMemberDisplayName(int groupMemberId) =>
        Members.FirstOrDefault(m => m.Id == groupMemberId)?.Nickname
        ?? Members.FirstOrDefault(m => m.Id == groupMemberId)?.AppUser?.DisplayName
        ?? "?";
}
