using CheapHelpers.Models.Contracts;

namespace CheapNights.Models;

public class MemberGamePlatform : IEntityId
{
    public int Id { get; set; }

    public int GroupMemberId { get; set; }
    public GroupMember? GroupMember { get; set; }

    public int GameEntryId { get; set; }
    public GameEntry? GameEntry { get; set; }

    public int PlatformId { get; set; }
    public Platform? Platform { get; set; }
}
