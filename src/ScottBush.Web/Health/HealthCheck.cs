namespace ScottBush.Web.Health;

/// <summary>
/// GET /healthz: 200 with the build's commit, so a deploy can wait until the public site is
/// serving the new release. The site has no database or dependencies, so running is healthy.
/// </summary>
public static class HealthCheck
{
    public static IResult Handle(HttpContext http)
    {
        http.Response.Headers.CacheControl = "no-store";
        http.Response.Headers["X-Robots-Tag"] = "noindex";
        return Results.Json(new { status = "ok", commit = BuildInfo.Commit });
    }
}
