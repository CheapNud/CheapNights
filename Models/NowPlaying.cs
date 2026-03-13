namespace CheapNights.Models;

public class NowPlaying
{
    public int Id { get; set; }
    public int? GameEntryId { get; set; }
    public GameEntry? GameEntry { get; set; }
    public string? StatusNote { get; set; }
    public DateTime UpdatedAt { get; set; }
}
