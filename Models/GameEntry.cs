using CheapHelpers.Models.Contracts;

namespace CheapNights.Models;

public class GameEntry : IEntityId
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public required string EntryType { get; set; }
    public string? Protagonist { get; set; }
    public string? StoryEra { get; set; }
    public int? StatusId { get; set; }
    public Status? Status { get; set; }
    public int StarRating { get; set; }
    public string? LengthLabel { get; set; }
    public string? PlatformBrecht { get; set; }
    public string? PlatformPieter { get; set; }
    public bool IsCouchCoop { get; set; }
    public bool IsMovie { get; set; }
    public string? SortLabel { get; set; }
    public int SortOrder { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CompletedNote { get; set; }
    public string? GameNote { get; set; }
}
