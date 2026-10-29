using System.Reflection;

namespace ScottBush.Web.Health;

/// <summary>The commit this build came from, stamped by the deploy's -p:SourceRevisionId.</summary>
public static class BuildInfo
{
    public static readonly string Commit =
        typeof(BuildInfo).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion.Split('+') is [_, var sha] ? sha : "local";
}
