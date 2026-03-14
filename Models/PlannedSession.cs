using CheapHelpers.Models.Contracts;

namespace CheapNights.Models;

public class PlannedSession : IEntityId
{
    public int Id { get; set; }
    public DateTime ScheduledAt { get; set; }
    public required string Location { get; set; }
    public int? GameEntryId { get; set; }
    public GameEntry? GameEntry { get; set; }
    public string? CustomGame { get; set; }
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; }
}
