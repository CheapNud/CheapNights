using CheapHelpers.Models.Contracts;

namespace CheapNights.Models;

public class Platform : IEntityId
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int SortOrder { get; set; }
}
