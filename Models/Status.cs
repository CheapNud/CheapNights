using CheapHelpers.Models.Contracts;

namespace CheapNights.Models;

public class Status : IEntityId
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? ChipColor { get; set; }
    public string? ChipBackground { get; set; }
    public string? StripeColor { get; set; }
    public bool IsSelectable { get; set; } = true;
    public int SortOrder { get; set; }
}
