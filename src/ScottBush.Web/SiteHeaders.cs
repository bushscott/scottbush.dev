namespace ScottBush.Web;

/// <summary>Canonical host and security headers for every response.</summary>
public static class SiteHeaders
{
    public const string CanonicalHost = "scottbush.dev";

    public static Task Apply(HttpContext http, RequestDelegate next)
    {
        // www.scottbush.dev and the bare domain share one tunnel route; only one of them is the site.
        if (http.Request.Host.Host.Equals("www." + CanonicalHost, StringComparison.OrdinalIgnoreCase))
        {
            http.Response.StatusCode = StatusCodes.Status301MovedPermanently;
            http.Response.Headers.Location = $"https://{CanonicalHost}{http.Request.PathBase}{http.Request.Path}{http.Request.QueryString}";
            return Task.CompletedTask;
        }

        // Razor component pages only answer GET. Link-preview bots and uptime checks often send
        // HEAD first, so serve HEAD as GET with the body thrown away instead of a 405.
        if (HttpMethods.IsHead(http.Request.Method))
        {
            http.Request.Method = HttpMethods.Get;
            http.Response.Body = Stream.Null;
        }

        var headers = http.Response.Headers;
        headers.XContentTypeOptions = "nosniff";
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), interest-cohort=()";
        headers["Cross-Origin-Opener-Policy"] = "same-origin";
        // Everything is served from this origin. The two small inline scripts (display settings
        // before first paint, and the hero pick) are the reason for 'unsafe-inline'.
        headers.ContentSecurityPolicy =
            "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data:; font-src 'self'; connect-src 'self'; object-src 'none'; " +
            "base-uri 'self'; form-action 'self'; frame-ancestors 'none'";
        return next(http);
    }
}
