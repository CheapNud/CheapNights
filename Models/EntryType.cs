using CheapHelpers.Models.Contracts;

namespace CheapNights.Models;

public class EntryType : IEntityId
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int SortOrder { get; set; }
}
