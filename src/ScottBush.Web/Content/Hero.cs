namespace ScottBush.Web.Content;

/// <summary>A hero headline and the description that goes with it.</summary>
public sealed record HeroPair(string Headline, string Description)
{
    /// <summary>Longer headlines step down a size so they still fit in three lines.</summary>
    public bool IsLong => Headline.Length > 34;
}

/// <summary>
/// The five hero pairs. The server renders the first, which is what visitors without JavaScript
/// see; an inline script picks one per visit before first paint, never the same twice in a row.
/// Never a rotating carousel.
/// </summary>
public static class Heroes
{
    public static readonly IReadOnlyList<HeroPair> All =
    [
        new("I build software that stays up.",
            "Senior .NET engineer with fourteen years building products end to end, from the database to the screen, often as the only engineer on them."),
        new("Systems that finish before 6 AM.",
            "I rebuilt a nightly life and annuity batch to finish in one to two hours instead of six. Fourteen years of C# and .NET across the whole stack, mostly in financial services."),
        new("Software that works when nobody’s watching.",
            "Live dashboards, booking sites and nightly pipelines that keep running unattended. Senior .NET engineer, fourteen years, often the only engineer on the system."),
        new("I make old systems fast again.",
            "From .NET Framework 2.0 to .NET 8, and from 150 ms per record to 30. Fourteen years modernizing legacy systems in financial services."),
        new("Every claim here comes with proof.",
            "Before-and-after numbers from fourteen years of .NET work, and how I proved the ones that mattered."),
    ];

    public static HeroPair Default => All[0];
}
