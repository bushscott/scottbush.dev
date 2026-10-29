# scottbush.dev

The portfolio and résumé of Scott Bush, senior .NET engineer in Ankeny, Iowa. Live at [scottbush.dev](https://scottbush.dev).

## How it's built

- **.NET 10 Blazor, static server rendering only.** Every page is plain HTML from the server. No WebAssembly, no SignalR circuit, no framework JavaScript. The content reads completely with JavaScript off.
- **Progressive enhancement.** `wwwroot/js/site.js` adds the Display settings panel, the click-to-enlarge screenshot viewer and the Pi boot sequence. Two small inline scripts run before first paint: one applies saved display settings, one picks the hero headline.
- **One source for the résumé.** `Content/Resume.cs` feeds both the page and a print stylesheet that sets it on two Letter pages.
- **Accessibility is a requirement, not a pass.** WCAG 2.1 AA and Section 508, AAA contrast for body text, high-contrast and colourblind palettes, text size and reduced motion. See the statement in the site footer for what was tested and how.
- **Assets.** AVIF with WebP fallback at two widths, lazy-loaded with fixed dimensions. One self-hosted variable font with a metric-matched fallback.

## Run it

```sh
dotnet run --project src/ScottBush.Web
dotnet test
```

Review links pin each state: `?theme=light|dark`, `&contrast=high`, `&cvd=rg|by`, `&text=125|150`, `&motion=reduce`, and `&hero=1` through `&hero=5`.

## Hosting

A self-contained linux-arm64 build runs under systemd on a Raspberry Pi 4 (`deploy/scottbush.service`), listening on loopback only, with a memory cap so it can never starve the site it shares the Pi with. A Cloudflare Tunnel carries public traffic to it, so no port is open on the network. `deploy/pi-setup.sh` prepares the Pi once. Every push to `main` runs `.github/workflows/deploy.yml`: test, publish with the newest .NET 10 patch (the runtime ships inside the app), copy to the Pi over Tailscale, restart, wait for the public `/healthz` to report the new commit, then check that plain HTTP and www always land on `https://scottbush.dev`. It also redeploys the day after Patch Tuesday each month so runtime security fixes arrive without a code change.
