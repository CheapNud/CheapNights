using CheapHelpers.Models.Contracts;

namespace CheapNights.Models;

public class Category : IEntityCode
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string? BadgeColor { get; set; }
    public int SortOrder { get; set; }

    public int GroupId { get; set; }
    public Group? Group { get; set; }
}
