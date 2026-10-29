using Microsoft.AspNetCore.HttpOverrides;
using ScottBush.Web;
using ScottBush.Web.Components;
using ScottBush.Web.Health;

var builder = WebApplication.CreateBuilder(args);

// cloudflared, on this machine, passes on that the visitor used https, so HSTS and redirects
// keep it. Only loopback is trusted to say so.
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor;
    o.ForwardedForHeaderName = "CF-Connecting-IP";
});
builder.Services.AddHsts(o =>
{
    o.MaxAge = TimeSpan.FromDays(365);
    o.IncludeSubDomains = true;
    o.Preload = true;
});
builder.Services.AddRazorComponents();

var app = builder.Build();

app.UseForwardedHeaders();
app.Use(SiteHeaders.Apply);
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
// Explicit, so routing sees the method after SiteHeaders has turned HEAD into GET.
app.UseRouting();
app.UseAntiforgery();

app.MapGet("/healthz", HealthCheck.Handle);
app.MapStaticAssets();
app.MapRazorComponents<App>();

app.Run();

public partial class Program;
