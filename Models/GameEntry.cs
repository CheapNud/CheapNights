using CheapHelpers.Models.Contracts;

namespace CheapNights.Models;

public class GameEntry : IEntityId
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int EntryTypeId { get; set; }
    public EntryType? EntryType { get; set; }

    public string? Protagonist { get; set; }
    public string? StoryEra { get; set; }

    public int? StatusId { get; set; }
    public Status? Status { get; set; }

    public int StarRating { get; set; }
    public string? LengthLabel { get; set; }

    public int? PlatformBrechtId { get; set; }
    public Platform? PlatformBrecht { get; set; }

    public int? PlatformPieterId { get; set; }
    public Platform? PlatformPieter { get; set; }

    public bool IsCouchCoop { get; set; }
    public bool IsMovie { get; set; }
    public string? SortLabel { get; set; }
    public int SortOrder { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CompletedNote { get; set; }
    public string? GameNote { get; set; }
}
