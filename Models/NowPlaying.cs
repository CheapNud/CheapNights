using CheapHelpers.Models.Contracts;

namespace CheapNights.Models;

public class NowPlaying : IEntityId
{
    public int Id { get; set; }

    public int GroupId { get; set; }
    public Group? Group { get; set; }

    public int? GameEntryId { get; set; }
    public GameEntry? GameEntry { get; set; }
    public string? StatusNote { get; set; }
    public DateTime UpdatedAt { get; set; }
}
