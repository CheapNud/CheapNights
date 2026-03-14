namespace CheapNights.Constants;

public static class GameConstants
{
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
