using CheapNights.Models;

namespace CheapNights.Helpers;

public class CalendarCell
{
    public int Day { get; set; }
    public bool IsToday { get; set; }
    public bool IsWeekend { get; set; }
    public List<PlannedSession> Sessions { get; set; } = [];
}
