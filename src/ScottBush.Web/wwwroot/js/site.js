// Display settings, the screenshot viewer and the Pi boot sequence.
// Everything here is an enhancement: the page reads completely without this file.
(() => {
  // Only the home page has these parts; the error pages share this file but none of it.
  if (!document.getElementById("display")) return;
  const root = document.documentElement;

  // ───────── Display settings ─────────
  // The inline script in <head> already applied saved or ?query choices before first paint.
  // This part keeps the panel's radios in step and saves new choices.
  const btn = document.getElementById("display-btn");
  const panel = document.getElementById("display");
  const keys = ["theme", "contrast", "cvd", "text", "motion"];
  const store = {
    set(k, v) { try { localStorage.setItem("display-" + k, v); } catch {} }
  };
  keys.forEach(k => {
    const input = panel.querySelector(`input[name="${k}"][value="${root.dataset[k]}"]`);
    if (input) input.checked = true;
  });

  // The browser's toolbar colour follows a theme picked here, not only the device's.
  const themeMetas = [...document.querySelectorAll('meta[name="theme-color"]')];
  themeMetas.forEach(m => m.dataset.device = m.content);
  function syncThemeColor() {
    const forced = root.dataset.theme !== "system";
    const bg = getComputedStyle(root).getPropertyValue("--bg").trim();
    themeMetas.forEach(m => m.content = forced ? bg : m.dataset.device);
  }
  syncThemeColor();

  panel.addEventListener("change", e => {
    const { name, value } = e.target;
    root.dataset[name] = value;
    store.set(name, value);
    syncThemeColor();
  });
  function setOpen(open) {
    panel.hidden = !open;
    btn.setAttribute("aria-expanded", open);
    if (open) { panel.classList.remove("opening"); void panel.offsetWidth; panel.classList.add("opening"); panel.querySelector("input:checked").focus(); }
  }
  btn.addEventListener("click", () => setOpen(panel.hidden));
  document.addEventListener("keydown", e => { if (e.key === "Escape" && !panel.hidden) { setOpen(false); btn.focus(); } });
  document.addEventListener("click", e => { if (!panel.hidden && !panel.contains(e.target) && !btn.contains(e.target)) setOpen(false); });

  // ───────── Screenshot viewer ─────────
  // Click only. Each screenshot becomes a button that opens the full-size image in a native
  // <dialog>; Esc, Close and a backdrop click close it, and focus returns to the opener.
  const viewer = document.getElementById("viewer");
  const vSource = document.getElementById("viewer-avif");
  const vImg = document.getElementById("viewer-img"), vCap = document.getElementById("viewer-cap");
  document.querySelectorAll(".shot picture").forEach(pic => {
    const img = pic.querySelector("img");
    const cap = pic.closest("figure").querySelector("figcaption")?.textContent.trim() || "";
    const b = document.createElement("button");
    b.className = "zoom"; b.type = "button";
    b.setAttribute("aria-label", "Enlarge screenshot: " + (cap || img.alt));
    pic.replaceWith(b); b.append(pic);
    b.addEventListener("click", () => {
      vSource.srcset = img.dataset.fullAvif;
      vImg.src = img.dataset.fullWebp; vImg.alt = img.alt;
      vCap.textContent = cap;
      viewer.showModal();
    });
  });
  document.getElementById("viewer-close").addEventListener("click", () => viewer.close());
  viewer.addEventListener("click", e => { if (e.target === viewer) viewer.close(); });

  // ───────── Boot sequence ─────────
  // The page's one authored motion: each Pi powers on in turn. Skipped when motion is reduced,
  // and the markup already has every board on for visitors without JavaScript.
  const pis = document.getElementById("pis");
  const items = [...pis.children];
  const reduce = root.dataset.motion === "reduce" || matchMedia("(prefers-reduced-motion: reduce)").matches;
  if (!reduce) {
    items.forEach(li => li.classList.remove("on"));
    requestAnimationFrame(() => {
      pis.classList.add("booted");
      items.forEach((li, i) => setTimeout(() => li.classList.add("on"), 250 + i * 220));
    });
  } else {
    pis.classList.add("booted");
  }
})();
