using CheapHelpers.Models.Contracts;

namespace CheapNights.Models;

public class GroupMember : IEntityId
{
    public int Id { get; set; }

    public int GroupId { get; set; }
    public Group? Group { get; set; }

    public int AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    public string? Nickname { get; set; }
}
