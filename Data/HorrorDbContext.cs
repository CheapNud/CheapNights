using CheapNights.Models;
using Microsoft.EntityFrameworkCore;

namespace CheapNights.Data;

public class HorrorDbContext(DbContextOptions<HorrorDbContext> options) : DbContext(options)
{
    public DbSet<GameEntry> GameEntries => Set<GameEntry>();
    public DbSet<PlannedSession> PlannedSessions => Set<PlannedSession>();
    public DbSet<NowPlaying> NowPlaying => Set<NowPlaying>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NowPlaying>().HasData(
            new NowPlaying { Id = 1, GameEntryId = 1, StatusNote = "almost finished, Arc reached", UpdatedAt = new DateTime(2026, 3, 13, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<GameEntry>().HasData(
            // ═══════════════════════════════════════
            //              RESIDENT EVIL
            // ═══════════════════════════════════════

            new GameEntry
            {
                Id = 1, SortOrder = 0, Name = "RE9 REQUIEM", Category = "RE", EntryType = "Game",
                Protagonist = "Grace + Leon", StoryEra = "2026", StatusLabel = "Done",
                StarRating = 3, LengthLabel = "15h", PlatformPieter = "PS5",
                IsCompleted = true, SortLabel = "✓",
                GameNote = "Arc reached — almost done!"
            },
            new GameEntry
            {
                Id = 2, SortOrder = 1, Name = "RE7 BIOHAZARD", Category = "RE", EntryType = "Game",
                Protagonist = "Ethan Winters", StoryEra = "2017", StatusLabel = "Downloading",
                StarRating = 2, LengthLabel = "9h", PlatformPieter = "PS5",
                SortLabel = "1",
                GameNote = "Downloading now — playing tonight!"
            },
            new GameEntry
            {
                Id = 3, SortOrder = 2, Name = "RE8 VILLAGE", Category = "RE", EntryType = "Game",
                Protagonist = "Ethan Winters", StoryEra = "2021", StatusLabel = "Essential",
                StarRating = 2, LengthLabel = "10h", PlatformPieter = "PS5 ✅",
                SortLabel = "2",
                GameNote = "Play immediately after RE7 — direct sequel"
            },
            new GameEntry
            {
                Id = 4, SortOrder = 3, Name = "RE: DEATH ISLAND (2023)", Category = "RE", EntryType = "Movie",
                Protagonist = "Animated (CG)", StoryEra = "2015", StatusLabel = "Canon",
                StarRating = 1, LengthLabel = "1.5h", IsMovie = true,
                GameNote = "Leon, Chris, Jill, Claire & Rebecca reunite — set after Village"
            },
            new GameEntry
            {
                Id = 5, SortOrder = 4, Name = "RE5", Category = "RE", EntryType = "Game",
                Protagonist = "Chris + Sheva", StoryEra = "~2009", StatusLabel = "Essential",
                StarRating = 2, LengthLabel = "12h", PlatformBrecht = "Steam ✅",
                IsCouchCoop = true, SortLabel = "3",
                GameNote = "Closes the Wesker arc"
            },
            new GameEntry
            {
                Id = 6, SortOrder = 5, Name = "RE: DAMNATION (2012)", Category = "RE", EntryType = "Movie",
                Protagonist = "Animated (CG)", StoryEra = "~2011", StatusLabel = "Canon",
                StarRating = 1, LengthLabel = "1.5h", IsMovie = true,
                GameNote = "Leon in a bio-war zone — bridges RE5 and RE6"
            },
            new GameEntry
            {
                Id = 7, SortOrder = 6, Name = "RE: VENDETTA (2017)", Category = "RE", EntryType = "Movie",
                Protagonist = "Animated (CG)", StoryEra = "~2014", StatusLabel = "Canon",
                StarRating = 1, LengthLabel = "1.5h", IsMovie = true,
                GameNote = "Leon + Chris + Rebecca, set after RE6 — watch after RE6 recap"
            },
            new GameEntry
            {
                Id = 8, SortOrder = 7, Name = "RE: REVELATIONS", Category = "RE", EntryType = "Game",
                Protagonist = "Jill Valentine", StoryEra = "~2005", StatusLabel = "Recommended",
                StarRating = 2, LengthLabel = "12h", SortLabel = "3b",
                GameNote = "Jill + Chris on a cruise ship — fills the RE5→RE4 gap. Cheap on Steam."
            },
            new GameEntry
            {
                Id = 9, SortOrder = 8, Name = "RE4 REMAKE", Category = "RE", EntryType = "Game",
                Protagonist = "Leon S. Kennedy", StoryEra = "~2004", StatusLabel = "Essential",
                StarRating = 3, LengthLabel = "16h", SortLabel = "4",
                GameNote = "Leon at his best"
            },
            new GameEntry
            {
                Id = 10, SortOrder = 9, Name = "RE4R: SEPARATE WAYS", Category = "RE", EntryType = "DLC",
                Protagonist = "Ada Wong", StoryEra = "~2004", StatusLabel = "Essential",
                StarRating = 1, LengthLabel = "6h", SortLabel = "4+",
                GameNote = "Ada Wong's parallel story — recontextualizes RE4R. Play immediately after. ~6hrs."
            },
            new GameEntry
            {
                Id = 11, SortOrder = 10, Name = "RE: DEGENERATION (2008)", Category = "RE", EntryType = "Movie",
                Protagonist = "Animated (CG)", StoryEra = "~2005", StatusLabel = "Canon",
                StarRating = 1, LengthLabel = "1.5h", IsMovie = true,
                GameNote = "Leon + Claire reunite ~1yr after RE4"
            },
            new GameEntry
            {
                Id = 12, SortOrder = 11, Name = "RE: INFINITE DARKNESS (2021)", Category = "RE", EntryType = "Movie",
                Protagonist = "Netflix (CG)", StoryEra = "2006", StatusLabel = "Canon",
                StarRating = 1, LengthLabel = "2h", IsMovie = true,
                GameNote = "Netflix CG series — Leon & Claire, White House conspiracy"
            },
            new GameEntry
            {
                Id = 13, SortOrder = 12, Name = "RE: REVELATIONS 2", Category = "RE", EntryType = "Game",
                Protagonist = "Claire + Barry", StoryEra = "~2011", StatusLabel = "Recommended",
                StarRating = 2, LengthLabel = "12h", IsCouchCoop = true, SortLabel = "5b",
                GameNote = "Claire + Barry, prison camp horror — has co-op! Fills RE5→RE6 gap. Cheap on Steam."
            },
            new GameEntry
            {
                Id = 14, SortOrder = 13, Name = "RE3 REMAKE", Category = "RE", EntryType = "Game",
                Protagonist = "Jill Valentine", StoryEra = "1998", StatusLabel = "Recommended",
                StarRating = 1, LengthLabel = "6h", PlatformBrecht = "Steam ✅", SortLabel = "5",
                GameNote = "Raccoon City's final days"
            },
            new GameEntry
            {
                Id = 15, SortOrder = 14, Name = "RE2 REMAKE", Category = "RE", EntryType = "Game",
                Protagonist = "Leon + Claire", StoryEra = "1998", StatusLabel = "Essential",
                StarRating = 2, LengthLabel = "8h", SortLabel = "6",
                GameNote = "Leon's origin. Where it all began for him."
            },
            new GameEntry
            {
                Id = 16, SortOrder = 15, Name = "RE6", Category = "RE", EntryType = "Game",
                Protagonist = "Leon / Chris / Jake", StoryEra = "~2013", StatusLabel = "Skip",
                StarRating = 3, LengthLabel = "21h", SortLabel = "—",
                GameNote = "Watch a story recap instead"
            },
            new GameEntry
            {
                Id = 17, SortOrder = 16, Name = "RE RESISTANCE", Category = "RE", EntryType = "Game",
                Protagonist = "Various", StoryEra = "Non-canon", StatusLabel = "Skip",
                PlatformBrecht = "Steam", SortLabel = "—",
                GameNote = "Non-canon multiplayer, dead servers"
            },
            new GameEntry
            {
                Id = 18, SortOrder = 17, Name = "CODE VERONICA REMAKE", Category = "RE", EntryType = "Game",
                Protagonist = "Claire Redfield", StoryEra = "~2000", StatusLabel = "Upcoming",
                SortLabel = "🔜",
                GameNote = "Rumored 2027 — Claire's story"
            },
            new GameEntry
            {
                Id = 19, SortOrder = 18, Name = "RESIDENT EVIL (2026)", Category = "RE", EntryType = "Movie",
                Protagonist = "Live-action reboot", StoryEra = "Sep 2026", StatusLabel = "Upcoming",
                StarRating = 2, LengthLabel = "2h", IsMovie = true, SortLabel = "🔜",
                GameNote = "Dir. Zach Cregger (Barbarian) — courier caught in outbreak, original story inspired by early games"
            },
            new GameEntry
            {
                Id = 20, SortOrder = 19, Name = "RE1 REMAKE", Category = "RE", EntryType = "Game",
                Protagonist = "Chris / Jill", StoryEra = "1998", StatusLabel = "Upcoming",
                SortLabel = "🔜",
                GameNote = "Early dev — 4–7 years out"
            },

            // ═══════════════════════════════════════
            //              SILENT HILL
            // ═══════════════════════════════════════

            new GameEntry
            {
                Id = 21, SortOrder = 100, Name = "SH: THE SHORT MESSAGE", Category = "SH", EntryType = "Game",
                Protagonist = "New protagonist", StoryEra = "2024", StatusLabel = "Essential",
                StarRating = 1, LengthLabel = "2h", PlatformPieter = "PS5 — free now", SortLabel = "0",
                GameNote = "FREE on PS5 — 2-3hrs, no combat, psychological horror. Play this first as a warmup!"
            },
            new GameEntry
            {
                Id = 22, SortOrder = 101, Name = "SILENT HILL 2 REMAKE", Category = "SH", EntryType = "Game",
                Protagonist = "Psychological grief horror", StoryEra = "2024", StatusLabel = "Essential",
                StarRating = 2, LengthLabel = "10h", SortLabel = "1",
                GameNote = "Start here. The masterpiece."
            },
            new GameEntry
            {
                Id = 23, SortOrder = 102, Name = "RETURN TO SILENT HILL (2026)", Category = "SH", EntryType = "Movie",
                Protagonist = "Live-action", StoryEra = "2026", StatusLabel = "Plex",
                StarRating = 2, LengthLabel = "2h", IsMovie = true,
                GameNote = "Adapts SH2 — same story, compare both versions ★ On your Plex!"
            },
            new GameEntry
            {
                Id = 24, SortOrder = 103, Name = "SILENT HILL (2006) + REVELATION (2012)", Category = "SH", EntryType = "Movie",
                Protagonist = "Live-action duo", StoryEra = "2006/2012", StatusLabel = "Optional",
                StarRating = 2, LengthLabel = "4h", IsMovie = true,
                GameNote = "Own continuity, not canon to games — optional horror movie night"
            },
            new GameEntry
            {
                Id = 25, SortOrder = 104, Name = "SILENT HILL f", Category = "SH", EntryType = "Game",
                Protagonist = "1960s Japan, new setting", StoryEra = "2025", StatusLabel = "Essential",
                StarRating = 3, LengthLabel = "15h", SortLabel = "2",
                GameNote = "Brand new story, bold & different"
            },
            new GameEntry
            {
                Id = 26, SortOrder = 105, Name = "SH HOMECOMING", Category = "SH", EntryType = "Game",
                Protagonist = "Action-heavy, less atmospheric", StoryEra = "2008", StatusLabel = "Skip",
                SortLabel = "—",
                GameNote = "Western-developed, weakest entry"
            },
            new GameEntry
            {
                Id = 27, SortOrder = 106, Name = "SH: TOWNFALL", Category = "SH", EntryType = "Game",
                Protagonist = "PT-inspired indie horror", StoryEra = "2026", StatusLabel = "Upcoming",
                SortLabel = "🔜",
                GameNote = "Scotland, 1996, first-person — out this year"
            },
            new GameEntry
            {
                Id = 28, SortOrder = 107, Name = "SH1 REMAKE", Category = "SH", EntryType = "Game",
                Protagonist = "Original Spencer Mansion", StoryEra = "~2027", StatusLabel = "Upcoming",
                SortLabel = "🔜",
                GameNote = "Bloober Team, series origins"
            }
        );
    }
}
