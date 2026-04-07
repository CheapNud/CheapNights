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
            e.HasOne(g => g.Owner).WithMany().HasForeignKey(g => g.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
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
        //              LOOKUP TABLES
        // ═══════════════════════════════════════
        // Only standalone lookups are seeded. All runtime data (AppUsers, Groups,
        // GroupMembers, Categories, GameEntries, NowPlaying, MemberGamePlatforms)
        // is managed through the UI and Plex auth flow.

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
    }
}
