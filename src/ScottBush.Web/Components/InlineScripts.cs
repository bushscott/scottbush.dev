using Microsoft.AspNetCore.Components;

namespace ScottBush.Web.Components;

/// <summary>
/// The page's only inline scripts. Both must run before first paint, which is why they aren't in
/// site.js; keeping them here as markup keeps them in one reviewable place.
/// </summary>
public static class InlineScripts
{
    /// <summary>
    /// Applies saved display choices (or ?theme= and friends from review links) to &lt;html&gt;, so a
    /// dark-mode visitor never sees a light flash, and marks the page as having JavaScript.
    /// </summary>
    public static readonly MarkupString DisplaySettings = new("""
        <script>
          (() => {
            const root = document.documentElement, query = new URLSearchParams(location.search);
            root.classList.add("js");
            for (const k of ["theme", "contrast", "cvd", "text", "motion"]) {
              let v = query.get(k);
              if (!v) try { v = localStorage.getItem("display-" + k); } catch {}
              if (v) root.dataset[k] = v;
            }
          })();
        </script>
        """);

    /// <summary>
    /// Picks one hero pair per visit, never the same twice in a row. The server-rendered first
    /// pair is what visitors without JavaScript see; ?hero=1–5 pins one for review.
    /// </summary>
    public static readonly MarkupString HeroPicker = new("""
        <script>
          (() => {
            const heroes = JSON.parse(document.currentScript.parentElement.dataset.heroes);
            const pinned = Number(new URLSearchParams(location.search).get("hero"));
            let i = pinned >= 1 && pinned <= heroes.length ? pinned - 1 : Math.floor(Math.random() * heroes.length);
            if (!pinned) {
              let last = null;
              try { last = sessionStorage.getItem("hero-last"); } catch {}
              if (String(i) === last) i = (i + 1) % heroes.length;   // never the same one twice in a row
              try { sessionStorage.setItem("hero-last", String(i)); } catch {}
            }
            const h1 = document.getElementById("hero-title");
            h1.textContent = heroes[i].headline;
            h1.classList.toggle("long", heroes[i].isLong);
            document.getElementById("hero-lede").textContent = heroes[i].description;
          })();
        </script>
        """);
}
