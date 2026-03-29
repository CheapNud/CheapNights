using CheapHelpers.Models.Contracts;

namespace CheapNights.Models;

public class AppUser : IEntityId
{
    public int Id { get; set; }
    public string PlexUserId { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
