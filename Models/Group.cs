using CheapHelpers.Models.Contracts;

namespace CheapNights.Models;

public class Group : IEntityId
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? ThemeColor { get; set; }
    public string ThemePreset { get; set; } = "horror-dark";
    public string? IconName { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<GroupMember> Members { get; set; } = [];
}
