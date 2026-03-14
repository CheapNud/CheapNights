namespace CheapNights.Constants;

public static class GameConstants
{
    public static class Categories
    {
        public const string ResidentEvil = "RE";
        public const string SilentHill = "SH";
    }

    public static class EntryTypes
    {
        public const string Game = "Game";
        public const string Movie = "Movie";
        public const string DLC = "DLC";
    }

    public static class Status
    {
        public const string Essential = "Essential";
        public const string Recommended = "Recommended";
        public const string Done = "Done";
        public const string Downloading = "Downloading";
        public const string Skip = "Skip";
        public const string Upcoming = "Upcoming";
        public const string Canon = "Canon";
        public const string Plex = "Plex";
        public const string Optional = "Optional";

        public static readonly string[] All =
        [
            Essential, Recommended, Done, Downloading, Skip, Upcoming,
            Canon, Plex, Optional
        ];

        public static readonly string[] ExcludedFromSelection = [Skip, Upcoming];
    }

    public static class Platforms
    {
        public const string Steam = "Steam";
        public const string PS5 = "PS5";

        public static readonly string[] All = [Steam, PS5];
    }

    public static class Players
    {
        public const string Brecht = "Brecht";
        public const string Pieter = "Pieter";
        public const string BrechtKey = "brecht";
        public const string PieterKey = "pieter";
    }

    public static class ProgressNotes
    {
        public const string JustStarted = "Just Started";
        public const string Downloading = "Downloading";
        public const string Midway = "Midway";
        public const string AlmostThere = "Almost There";
        public const string Completed = "Completed!";
        public const string OnHold = "On Hold";
        public const string Watching = "Watching";
        public const string CoopReady = "Co-op Ready";

        public static readonly string[] All =
        [
            JustStarted, Downloading, Midway, AlmostThere, Completed,
            OnHold, Watching, CoopReady
        ];
    }

    public static class Defaults
    {
        public const int NowPlayingId = 1;
        public const int UpcomingSessionCount = 8;
        public const string DefaultCustomGame = "Game Night (TBD)";
        public static readonly TimeSpan DefaultSessionTime = new(20, 0, 0);
    }
}
