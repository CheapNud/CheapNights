using CheapNights.Models;
using Microsoft.EntityFrameworkCore;

namespace CheapNights.Data;

public class CheapNightsDbContext(DbContextOptions<CheapNightsDbContext> options) : DbContext(options)
{
    public DbSet<GameEntry> GameEntries => Set<GameEntry>();
    public DbSet<PlannedSession> PlannedSessions => Set<PlannedSession>();
    public DbSet<NowPlaying> NowPlaying => Set<NowPlaying>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<EntryType> EntryTypes => Set<EntryType>();
    public DbSet<Platform> Platforms => Set<Platform>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
    public DbSet<MemberGamePlatform> MemberGamePlatforms => Set<MemberGamePlatform>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ═══════════════════════════════════════
        //              RELATIONSHIPS
        // ═══════════════════════════════════════

        modelBuilder.Entity<GameEntry>(e =>
        {
            e.HasOne(g => g.Status).WithMany().HasForeignKey(g => g.StatusId);
            e.HasOne(g => g.Category).WithMany().HasForeignKey(g => g.CategoryId);
            e.HasOne(g => g.EntryType).WithMany().HasForeignKey(g => g.EntryTypeId);
            e.HasOne(g => g.Group).WithMany().HasForeignKey(g => g.GroupId);
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.HasOne(c => c.Group).WithMany().HasForeignKey(c => c.GroupId);
        });

        modelBuilder.Entity<PlannedSession>(e =>
        {
            e.HasOne(s => s.GameEntry).WithMany().HasForeignKey(s => s.GameEntryId);
            e.HasOne(s => s.Group).WithMany().HasForeignKey(s => s.GroupId);
            e.HasOne(s => s.HostMember).WithMany().HasForeignKey(s => s.HostMemberId);
        });

        modelBuilder.Entity<NowPlaying>(e =>
        {
            e.HasOne(n => n.GameEntry).WithMany().HasForeignKey(n => n.GameEntryId);
            e.HasOne(n => n.Group).WithMany().HasForeignKey(n => n.GroupId);
            e.HasIndex(n => n.GroupId).IsUnique();
        });

        modelBuilder.Entity<Group>(e =>
        {
            e.HasOne(g => g.Owner).WithMany().HasForeignKey(g => g.OwnerId);
        });

        modelBuilder.Entity<GroupMember>(e =>
        {
            e.HasOne(m => m.Group).WithMany(g => g.Members).HasForeignKey(m => m.GroupId);
            e.HasOne(m => m.AppUser).WithMany().HasForeignKey(m => m.AppUserId);
            e.HasIndex(m => new { m.GroupId, m.AppUserId }).IsUnique();
        });

        modelBuilder.Entity<MemberGamePlatform>(e =>
        {
            e.HasOne(p => p.GroupMember).WithMany().HasForeignKey(p => p.GroupMemberId);
            e.HasOne(p => p.GameEntry).WithMany(g => g.MemberPlatforms).HasForeignKey(p => p.GameEntryId);
            e.HasOne(p => p.Platform).WithMany().HasForeignKey(p => p.PlatformId);
            e.HasIndex(p => new { p.GroupMemberId, p.GameEntryId }).IsUnique();
        });

        // ═══════════════════════════════════════
        //              APP USERS
        // ═══════════════════════════════════════

        modelBuilder.Entity<AppUser>().HasData(
            new AppUser { Id = 1, PlexUserId = "brecht", DisplayName = "Brecht", CreatedAt = new DateTime(2026, 3, 14, 0, 0, 0, DateTimeKind.Utc) },
            new AppUser { Id = 2, PlexUserId = "pieter", DisplayName = "Pieter", CreatedAt = new DateTime(2026, 3, 14, 0, 0, 0, DateTimeKind.Utc) }
        );

        // ═══════════════════════════════════════
        //              GROUPS
        // ═══════════════════════════════════════

        modelBuilder.Entity<Group>().HasData(
            new Group { Id = 1, Name = "Horror Nights", Description = "Survival horror roadmap — Resident Evil & Silent Hill", ThemeColor = "#c0392b", ThemePreset = "horror-dark", IconName = "Theaters", OwnerId = 1, CreatedAt = new DateTime(2026, 3, 14, 0, 0, 0, DateTimeKind.Utc) },
            new Group { Id = 2, Name = "Schedule 1", Description = "Schedule 1 gaming sessions", ThemeColor = "#4caf50", ThemePreset = "forest", IconName = "SportsEsports", OwnerId = 1, CreatedAt = new DateTime(2026, 3, 29, 0, 0, 0, DateTimeKind.Utc) }
        );

        // ═══════════════════════════════════════
        //              GROUP MEMBERS
        // ═══════════════════════════════════════
        // GroupMember 1: Brecht in Horror Nights
        // GroupMember 2: Pieter in Horror Nights
        // GroupMember 3: Brecht in Schedule 1

        modelBuilder.Entity<GroupMember>().HasData(
            new GroupMember { Id = 1, GroupId = 1, AppUserId = 1, Nickname = "brecht" },
            new GroupMember { Id = 2, GroupId = 1, AppUserId = 2, Nickname = "pieter" },
            new GroupMember { Id = 3, GroupId = 2, AppUserId = 1, Nickname = "brecht" }
        );

        // ═══════════════════════════════════════
        //              LOOKUP TABLES
        // ═══════════════════════════════════════

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Code = "RE", Name = "Resident Evil", BadgeColor = "re", SortOrder = 0, GroupId = 1 },
            new Category { Id = 2, Code = "SH", Name = "Silent Hill", BadgeColor = "sh", SortOrder = 1, GroupId = 1 },
            new Category { Id = 3, Code = "S1", Name = "Schedule 1", BadgeColor = "s1", SortOrder = 0, GroupId = 2 }
        );

        modelBuilder.Entity<EntryType>().HasData(
            new EntryType { Id = 1, Name = "Game", SortOrder = 0 },
            new EntryType { Id = 2, Name = "Movie", SortOrder = 1 },
            new EntryType { Id = 3, Name = "DLC", SortOrder = 2 }
        );

        modelBuilder.Entity<Platform>().HasData(
            new Platform { Id = 1, Name = "Steam", SortOrder = 0 },
            new Platform { Id = 2, Name = "PS5", SortOrder = 1 },
            new Platform { Id = 3, Name = "Plex", SortOrder = 2 }
        );

        modelBuilder.Entity<Status>().HasData(
            new Status { Id = 1, Name = "Essential", ChipColor = "#e05a4a", ChipBackground = "rgba(192,57,43,0.1)", StripeColor = "var(--re-red)", SortOrder = 0 },
            new Status { Id = 2, Name = "Recommended", ChipColor = "#888", ChipBackground = "rgba(150,150,150,0.08)", StripeColor = "#888", SortOrder = 1 },
            new Status { Id = 3, Name = "Optional", ChipColor = "#666", ChipBackground = "rgba(110,110,110,0.07)", StripeColor = null, SortOrder = 2 },
            new Status { Id = 4, Name = "Skip", ChipColor = "#444", ChipBackground = null, StripeColor = "var(--skip-grey)", SortOrder = 3 },
            new Status { Id = 5, Name = "Upcoming", ChipColor = "#a98be8", ChipBackground = "rgba(124,92,191,0.1)", StripeColor = "var(--upcoming)", SortOrder = 4 }
        );

        // ═══════════════════════════════════════
        //              NOW PLAYING (per group)
        // ═══════════════════════════════════════

        modelBuilder.Entity<NowPlaying>().HasData(
            new NowPlaying { Id = 1, GroupId = 1, GameEntryId = 1, StatusNote = "almost finished, Arc reached", UpdatedAt = new DateTime(2026, 3, 13, 0, 0, 0, DateTimeKind.Utc) },
            new NowPlaying { Id = 2, GroupId = 2, GameEntryId = null, StatusNote = null, UpdatedAt = new DateTime(2026, 3, 29, 0, 0, 0, DateTimeKind.Utc) }
        );

        // ═══════════════════════════════════════
        //     MEMBER GAME PLATFORMS
        //     (migrated from PlatformBrechtId/PieterId)
        // ═══════════════════════════════════════
        // Brecht (GroupMemberId=1): RE5→Steam, RE3R→Steam, RE Resistance→Steam, Return to SH→Plex
        // Pieter (GroupMemberId=2): RE9→PS5, RE7→PS5, RE8→PS5, SH Short Message→PS5

        modelBuilder.Entity<MemberGamePlatform>().HasData(
            new MemberGamePlatform { Id = 1, GroupMemberId = 1, GameEntryId = 5, PlatformId = 1 },
            new MemberGamePlatform { Id = 2, GroupMemberId = 1, GameEntryId = 14, PlatformId = 1 },
            new MemberGamePlatform { Id = 3, GroupMemberId = 1, GameEntryId = 17, PlatformId = 1 },
            new MemberGamePlatform { Id = 4, GroupMemberId = 1, GameEntryId = 23, PlatformId = 3 },
            new MemberGamePlatform { Id = 5, GroupMemberId = 2, GameEntryId = 1, PlatformId = 2 },
            new MemberGamePlatform { Id = 6, GroupMemberId = 2, GameEntryId = 2, PlatformId = 2 },
            new MemberGamePlatform { Id = 7, GroupMemberId = 2, GameEntryId = 3, PlatformId = 2 },
            new MemberGamePlatform { Id = 8, GroupMemberId = 2, GameEntryId = 21, PlatformId = 2 }
        );

        // ═══════════════════════════════════════
        //     RESIDENT EVIL  (CategoryId=1, GroupId=1)
        // ═══════════════════════════════════════
        // EntryTypeId: 1=Game, 2=Movie, 3=DLC
        // StatusId: 1=Essential, 2=Recommended, 3=Optional, 4=Skip, 5=Upcoming
        // PlatformId: 1=Steam, 2=PS5, 3=Plex

        modelBuilder.Entity<GameEntry>().HasData(
            new GameEntry { Id = 1, GroupId = 1, SortOrder = 0, Name = "RE9 REQUIEM", CategoryId = 1, EntryTypeId = 1, Protagonist = "Grace + Leon", StoryEra = "2026", StatusId = 1, StarRating = 3, LengthLabel = "15h", IsCompleted = true, SortLabel = "✓", GameNote = "Grace & Leon face a new Megamycete threat — series finale, wraps up the Winters saga" },
            new GameEntry { Id = 2, GroupId = 1, SortOrder = 1, Name = "RE7 BIOHAZARD", CategoryId = 1, EntryTypeId = 1, Protagonist = "Ethan Winters", StoryEra = "2017", StatusId = 1, StarRating = 2, LengthLabel = "9h", SortLabel = "1", GameNote = "Ethan searches for Mia in the Baker estate — first-person horror, soft reboot of the series" },
            new GameEntry { Id = 3, GroupId = 1, SortOrder = 2, Name = "RE8 VILLAGE", CategoryId = 1, EntryTypeId = 1, Protagonist = "Ethan Winters", StoryEra = "2021", StatusId = 1, StarRating = 2, LengthLabel = "10h", SortLabel = "2", GameNote = "Play immediately after RE7 — direct sequel" },
            new GameEntry { Id = 4, GroupId = 1, SortOrder = 3, Name = "RE: DEATH ISLAND (2023)", CategoryId = 1, EntryTypeId = 2, Protagonist = "Animated (CG)", StoryEra = "2015", StatusId = 2, StarRating = 1, LengthLabel = "1.5h", IsMovie = true, GameNote = "Leon, Chris, Jill, Claire & Rebecca reunite — set after Village" },
            new GameEntry { Id = 5, GroupId = 1, SortOrder = 4, Name = "RE5", CategoryId = 1, EntryTypeId = 1, Protagonist = "Chris + Sheva", StoryEra = "2009", StatusId = 1, StarRating = 2, LengthLabel = "12h", IsCouchCoop = true, SortLabel = "3", GameNote = "Closes the Wesker arc" },
            new GameEntry { Id = 6, GroupId = 1, SortOrder = 5, Name = "RE: DAMNATION (2012)", CategoryId = 1, EntryTypeId = 2, Protagonist = "Animated (CG)", StoryEra = "2011", StatusId = 2, StarRating = 1, LengthLabel = "1.5h", IsMovie = true, GameNote = "Leon in a bio-war zone — bridges RE5 and RE6" },
            new GameEntry { Id = 7, GroupId = 1, SortOrder = 6, Name = "RE: VENDETTA (2017)", CategoryId = 1, EntryTypeId = 2, Protagonist = "Animated (CG)", StoryEra = "2014", StatusId = 2, StarRating = 1, LengthLabel = "1.5h", IsMovie = true, GameNote = "Leon + Chris + Rebecca, set after RE6 — watch after RE6 recap" },
            new GameEntry { Id = 8, GroupId = 1, SortOrder = 7, Name = "RE: REVELATIONS", CategoryId = 1, EntryTypeId = 1, Protagonist = "Jill Valentine", StoryEra = "2005", StatusId = 2, StarRating = 2, LengthLabel = "12h", SortLabel = "3b", GameNote = "Jill + Chris on a cruise ship — fills the RE5→RE4 gap" },
            new GameEntry { Id = 9, GroupId = 1, SortOrder = 8, Name = "RE4 REMAKE", CategoryId = 1, EntryTypeId = 1, Protagonist = "Leon S. Kennedy", StoryEra = "2004", StatusId = 1, StarRating = 3, LengthLabel = "16h", SortLabel = "4", GameNote = "Leon at his best" },
            new GameEntry { Id = 10, GroupId = 1, SortOrder = 9, Name = "RE4R: SEPARATE WAYS", CategoryId = 1, EntryTypeId = 3, Protagonist = "Ada Wong", StoryEra = "2004", StatusId = 1, StarRating = 1, LengthLabel = "6h", SortLabel = "4+", GameNote = "Ada Wong's parallel story — recontextualizes RE4R. Play immediately after." },
            new GameEntry { Id = 11, GroupId = 1, SortOrder = 10, Name = "RE: DEGENERATION (2008)", CategoryId = 1, EntryTypeId = 2, Protagonist = "Animated (CG)", StoryEra = "2005", StatusId = 2, StarRating = 1, LengthLabel = "1.5h", IsMovie = true, GameNote = "Leon + Claire reunite ~1yr after RE4" },
            new GameEntry { Id = 12, GroupId = 1, SortOrder = 11, Name = "RE: INFINITE DARKNESS (2021)", CategoryId = 1, EntryTypeId = 2, Protagonist = "Netflix (CG)", StoryEra = "2006", StatusId = 2, StarRating = 1, LengthLabel = "2h", IsMovie = true, GameNote = "Netflix CG series — Leon & Claire, White House conspiracy" },
            new GameEntry { Id = 13, GroupId = 1, SortOrder = 12, Name = "RE: REVELATIONS 2", CategoryId = 1, EntryTypeId = 1, Protagonist = "Claire + Barry", StoryEra = "2011", StatusId = 2, StarRating = 2, LengthLabel = "12h", IsCouchCoop = true, SortLabel = "5b", GameNote = "Claire + Barry, prison camp horror — has co-op!" },
            new GameEntry { Id = 14, GroupId = 1, SortOrder = 13, Name = "RE3 REMAKE", CategoryId = 1, EntryTypeId = 1, Protagonist = "Jill Valentine", StoryEra = "1998", StatusId = 2, StarRating = 1, LengthLabel = "6h", SortLabel = "5", GameNote = "Raccoon City's final days" },
            new GameEntry { Id = 15, GroupId = 1, SortOrder = 14, Name = "RE2 REMAKE", CategoryId = 1, EntryTypeId = 1, Protagonist = "Leon + Claire", StoryEra = "1998", StatusId = 1, StarRating = 2, LengthLabel = "8h", SortLabel = "6", GameNote = "Leon's origin. Where it all began for him." },
            new GameEntry { Id = 16, GroupId = 1, SortOrder = 15, Name = "RE6", CategoryId = 1, EntryTypeId = 1, Protagonist = "Leon / Chris / Jake", StoryEra = "2013", StatusId = 4, StarRating = 3, LengthLabel = "21h", SortLabel = "—", GameNote = "Watch a story recap instead" },
            new GameEntry { Id = 17, GroupId = 1, SortOrder = 16, Name = "RE RESISTANCE", CategoryId = 1, EntryTypeId = 1, Protagonist = "Various", StoryEra = "", StatusId = 4, SortLabel = "—", GameNote = "Non-canon multiplayer, dead servers" },
            new GameEntry { Id = 18, GroupId = 1, SortOrder = 17, Name = "CODE VERONICA REMAKE", CategoryId = 1, EntryTypeId = 1, Protagonist = "Claire Redfield", StoryEra = "2000", StatusId = 5, SortLabel = "🔜", GameNote = "Rumored 2027 — Claire's story" },
            new GameEntry { Id = 19, GroupId = 1, SortOrder = 18, Name = "RESIDENT EVIL (2026)", CategoryId = 1, EntryTypeId = 2, Protagonist = "Live-action reboot", StoryEra = "2026", StatusId = 5, StarRating = 2, LengthLabel = "2h", IsMovie = true, SortLabel = "🔜", GameNote = "Dir. Zach Cregger (Barbarian)" },
            new GameEntry { Id = 20, GroupId = 1, SortOrder = 19, Name = "RE1 REMAKE", CategoryId = 1, EntryTypeId = 1, Protagonist = "Chris / Jill", StoryEra = "1998", StatusId = 5, SortLabel = "🔜", GameNote = "Early dev — 4–7 years out" },

            // ═══════════════════════════════════════
            //         SILENT HILL  (CategoryId=2, GroupId=1)
            // ═══════════════════════════════════════

            new GameEntry { Id = 21, GroupId = 1, SortOrder = 100, Name = "SH: THE SHORT MESSAGE", CategoryId = 2, EntryTypeId = 1, Protagonist = "New protagonist", StoryEra = "2024", StatusId = 1, StarRating = 1, LengthLabel = "2h", SortLabel = "0", GameNote = "FREE on PS5 — standalone intro to Silent Hill themes, modern setting with a new protagonist" },
            new GameEntry { Id = 22, GroupId = 1, SortOrder = 101, Name = "SILENT HILL 2 REMAKE", CategoryId = 2, EntryTypeId = 1, Protagonist = "Psychological grief horror", StoryEra = "2024", StatusId = 1, StarRating = 2, LengthLabel = "10h", SortLabel = "1", GameNote = "James Sunderland returns to Silent Hill after receiving a letter from his dead wife — psychological horror masterpiece" },
            new GameEntry { Id = 23, GroupId = 1, SortOrder = 102, Name = "RETURN TO SILENT HILL (2026)", CategoryId = 2, EntryTypeId = 2, Protagonist = "Live-action", StoryEra = "2026", StatusId = 2, StarRating = 2, LengthLabel = "2h", IsMovie = true, GameNote = "Live-action adaptation of SH2 — directed by Christophe Gans, compare with the game version" },
            new GameEntry { Id = 24, GroupId = 1, SortOrder = 103, Name = "SILENT HILL (2006) + REVELATION (2012)", CategoryId = 2, EntryTypeId = 2, Protagonist = "Live-action duo", StoryEra = "2006", StatusId = 3, StarRating = 2, LengthLabel = "4h", IsMovie = true, GameNote = "Own continuity loosely adapting SH1 + SH3 — not canon to games, optional horror movie night" },
            new GameEntry { Id = 25, GroupId = 1, SortOrder = 104, Name = "SILENT HILL f", CategoryId = 2, EntryTypeId = 1, Protagonist = "1960s Japan, new setting", StoryEra = "2025", StatusId = 1, StarRating = 3, LengthLabel = "15h", SortLabel = "2", GameNote = "Set in 1960s Japan with a completely new story — bold departure from Americana fog, kowloon-style horror" },
            new GameEntry { Id = 26, GroupId = 1, SortOrder = 105, Name = "SH HOMECOMING", CategoryId = 2, EntryTypeId = 1, Protagonist = "Action-heavy, less atmospheric", StoryEra = "2008", StatusId = 4, SortLabel = "—", GameNote = "Alex Shepherd returns to Shepherd's Glen — combat-focused, Western-developed, widely considered weakest entry" },
            new GameEntry { Id = 27, GroupId = 1, SortOrder = 106, Name = "SH: TOWNFALL", CategoryId = 2, EntryTypeId = 1, Protagonist = "PT-inspired indie horror", StoryEra = "2026", StatusId = 5, SortLabel = "🔜", GameNote = "By No Code (Stories Untold) — Scotland 1996, first-person, P.T.-inspired indie horror set in the SH universe" },
            new GameEntry { Id = 28, GroupId = 1, SortOrder = 107, Name = "SH1 REMAKE", CategoryId = 2, EntryTypeId = 1, Protagonist = "Original Spencer Mansion", StoryEra = "2027", StatusId = 5, SortLabel = "🔜", GameNote = "Bloober Team remaking the original — Harry Mason searching for his daughter Cheryl, the story that started it all" }
        );
    }
}
