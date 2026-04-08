using System.Text;
using CheapNights.Models;
using CheapNights.Repositories;

namespace CheapNights.Helpers;

public static class CalendarEndpoints
{
    public static void MapCalendarEndpoints(this WebApplication app)
    {
        app.MapGet("/api/calendar/{token}.ics", async (Guid token, AppUserRepo appUserRepo, SessionRepo sessionRepo) =>
        {
            var appUser = await appUserRepo.GetByCalendarTokenAsync(token);
            if (appUser is null)
                return Results.NotFound();

            var sessions = await sessionRepo.GetAllForUserAsync(appUser.Id);
            var ical = BuildICalendar(sessions, appUser.DisplayName);

            return Results.Text(ical, "text/calendar; charset=utf-8");
        }).AllowAnonymous();
    }

    private static string BuildICalendar(List<PlannedSession> sessions, string userName)
    {
        var sb = new StringBuilder();
        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//CheapNights//EN");
        sb.AppendLine($"X-WR-CALNAME:CheapNights — {EscapeIcal(userName)}");
        sb.AppendLine("CALSCALE:GREGORIAN");
        sb.AppendLine("METHOD:PUBLISH");

        foreach (var session in sessions)
        {
            var gameName = ResolveGameName(session);
            var groupName = session.Group?.Name ?? "CheapNights";
            var hostName = session.HostMember?.AppUser?.DisplayName
                        ?? session.HostMember?.Nickname;

            var summary = hostName is not null
                ? $"{gameName} @ {hostName}'s"
                : gameName;

            var descriptionParts = new List<string> { groupName };
            if (!string.IsNullOrEmpty(session.Notes))
                descriptionParts.Add(session.Notes);

            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"UID:cheapnights-session-{session.Id}@cheapnights");
            sb.AppendLine($"DTSTART:{FormatDateTime(session.ScheduledAt)}");
            sb.AppendLine($"DTEND:{FormatDateTime(session.ScheduledAt.AddHours(3))}");
            sb.AppendLine($"SUMMARY:{EscapeIcal(summary)}");
            sb.AppendLine($"DESCRIPTION:{EscapeIcal(string.Join(" — ", descriptionParts))}");
            if (hostName is not null)
                sb.AppendLine($"LOCATION:{EscapeIcal(hostName)}");
            sb.AppendLine($"DTSTAMP:{FormatDateTime(DateTime.UtcNow)}");
            sb.AppendLine("END:VEVENT");
        }

        sb.AppendLine("END:VCALENDAR");
        return sb.ToString();
    }

    private static string ResolveGameName(PlannedSession session)
    {
        if (!string.IsNullOrEmpty(session.CustomGame))
            return session.CustomGame;
        if (session.GameEntry is not null)
            return session.GameEntry.Name;
        return "Game Night";
    }

    private static string FormatDateTime(DateTime dt) =>
        dt.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");

    private static string EscapeIcal(string text) =>
        text.Replace("\\", "\\\\").Replace(",", "\\,").Replace(";", "\\;").Replace("\n", "\\n");
}
