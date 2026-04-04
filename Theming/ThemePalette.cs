namespace CheapNights.Theming;

public record ThemePalette
{
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
    public required bool IsDarkMode { get; init; }

    // MudBlazor palette colors
    public required string Primary { get; init; }
    public required string Secondary { get; init; }
    public required string Background { get; init; }
    public required string Surface { get; init; }
    public required string AppbarBackground { get; init; }
    public required string TextPrimary { get; init; }
    public required string TextSecondary { get; init; }
    public required string DrawerBackground { get; init; }
    public required string DrawerText { get; init; }
    public required string ActionDefault { get; init; }
    public required string ActionDisabled { get; init; }
    public required string Dark { get; init; }

    // CSS custom properties
    public required string Surface2 { get; init; }
    public required string Border { get; init; }
    public required string TextMid { get; init; }
    public required string GradientStart { get; init; }
    public required string GradientMid { get; init; }
    public required string GradientEnd { get; init; }
    public bool ShowScanlines { get; init; }
}
