using System.Globalization;
using System.Security.Claims;
using System.Text;
using CheapNights.Models;
using CheapNights.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace CheapNights.Helpers;

public static class CalendarEndpoints
{
    private const string Crlf = "\r\n";

    public static void MapCalendarEndpoints(this WebApplication app)
    {
        app.MapGet("/api/calendar/{token}.ics", async (Guid token, HttpContext context, AppUserRepo appUserRepo, SessionRepo sessionRepo) =>
        {
            var appUser = await appUserRepo.GetByCalendarTokenAsync(token);
            if (appUser is null)
                return Results.NotFound();

            var sessions = await sessionRepo.GetAllForUserAsync(appUser.Id);
            var ical = BuildICalendar(sessions, appUser.DisplayName);

            context.Response.Headers.CacheControl = "private, no-store";
            context.Response.Headers.Pragma = "no-cache";

            return Results.Text(ical, "text/calendar; charset=utf-8");
        })
        .AllowAnonymous()
        .RequireRateLimiting("calendar");

        app.MapGet("/api/calendar/export.csv", [Authorize] async (HttpContext context, AppUserRepo appUserRepo, SessionRepo sessionRepo) =>
        {
            var plexId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (plexId is null)
                return Results.Unauthorized();

            var appUser = await appUserRepo.GetByPlexIdAsync(plexId);
            if (appUser is null)
                return Results.Unauthorized();

            var sessions = await sessionRepo.GetAllForUserAsync(appUser.Id);
            var csv = BuildCsv(sessions);
            var fileName = $"cheapnights-sessions-{DateTime.UtcNow:yyyy-MM-dd}.csv";

            return Results.File(Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
        });
    }

    private static string BuildCsv(List<PlannedSession> sessions)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Date,Time,Game,Host,Group,Notes");

        foreach (var session in sessions)
        {
            var local = session.ScheduledAt.ToLocalTime();
            var date = local.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var time = local.ToString("HH:mm", CultureInfo.InvariantCulture);
            var game = ResolveGameName(session);
            var host = session.HostMember?.AppUser?.DisplayName
                    ?? session.HostMember?.Nickname
                    ?? "";
            var group = session.Group?.Name ?? "";
            var notes = session.Notes ?? "";

            sb.Append(EscapeCsv(date)).Append(',');
            sb.Append(EscapeCsv(time)).Append(',');
            sb.Append(EscapeCsv(game)).Append(',');
            sb.Append(EscapeCsv(host)).Append(',');
            sb.Append(EscapeCsv(group)).Append(',');
            sb.AppendLine(EscapeCsv(notes));
        }

        return sb.ToString();
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    private static string BuildICalendar(List<PlannedSession> sessions, string userName)
    {
        var sb = new StringBuilder();
        AppendLine(sb, "BEGIN:VCALENDAR");
        AppendLine(sb, "VERSION:2.0");
        AppendLine(sb, "PRODID:-//CheapNights//EN");
        AppendFolded(sb, $"X-WR-CALNAME:CheapNights — {EscapeIcal(userName)}");
        AppendLine(sb, "CALSCALE:GREGORIAN");
        AppendLine(sb, "METHOD:PUBLISH");

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

            AppendLine(sb, "BEGIN:VEVENT");
            AppendLine(sb, $"UID:cheapnights-session-{session.Id}@cheapnights");
            AppendLine(sb, $"DTSTART:{FormatDateTime(session.ScheduledAt)}");
            AppendLine(sb, $"DTEND:{FormatDateTime(session.ScheduledAt.AddHours(3))}");
            AppendFolded(sb, $"SUMMARY:{EscapeIcal(summary)}");
            AppendFolded(sb, $"DESCRIPTION:{EscapeIcal(string.Join(" — ", descriptionParts))}");
            if (hostName is not null)
                AppendFolded(sb, $"LOCATION:{EscapeIcal(hostName)}");
            AppendLine(sb, $"DTSTAMP:{FormatDateTime(DateTime.UtcNow)}");
            AppendLine(sb, "END:VEVENT");
        }

        AppendLine(sb, "END:VCALENDAR");
        return sb.ToString();
    }

    private static void AppendLine(StringBuilder sb, string line) =>
        sb.Append(line).Append(Crlf);

    private static void AppendFolded(StringBuilder sb, string line)
    {
        if (Encoding.UTF8.GetByteCount(line) <= 75)
        {
            sb.Append(line).Append(Crlf);
            return;
        }

        var bytes = Encoding.UTF8.GetBytes(line);
        var offset = 0;
        var first = true;

        while (offset < bytes.Length)
        {
            var maxChunk = first ? 75 : 74; // continuation lines start with a space
            var chunkLen = Math.Min(maxChunk, bytes.Length - offset);

            // don't split in the middle of a multi-byte UTF-8 sequence
            while (chunkLen > 1 && (bytes[offset + chunkLen - 1] & 0xC0) == 0x80)
                chunkLen--;

            if (!first)
                sb.Append(' ');

            sb.Append(Encoding.UTF8.GetString(bytes, offset, chunkLen)).Append(Crlf);
            offset += chunkLen;
            first = false;
        }
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
        text.Replace("\\", "\\\\").Replace(",", "\\,").Replace(";", "\\;")
            .Replace("\r\n", "\\n").Replace("\r", "\\n").Replace("\n", "\\n");
}
