using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using ScottBush.Web.Content;

namespace ScottBush.Web.Tests;

/// <summary>
/// The page's content rules, checked against the rendered HTML: what the site must always say,
/// and what it must never say.
/// </summary>
public sealed partial class SiteTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient(new() { AllowAutoRedirect = false });

    private async Task<string> HomeAsync()
    {
        var response = await _client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.Content.ReadAsStringAsync();
    }

    [Fact]
    public async Task Without_javascript_the_first_hero_pair_is_in_the_html()
    {
        var html = await HomeAsync();
        Assert.Contains($">{Heroes.Default.Headline}</h1>", html);
        Assert.Contains(Heroes.Default.Description, html);
    }

    [Fact]
    public async Task All_five_hero_pairs_reach_the_picker()
    {
        var html = WebUtility.HtmlDecode(await HomeAsync());
        foreach (var hero in Heroes.All)
            Assert.Contains(hero.Headline.Replace("\u00a0", "\\u00A0").Replace("\u2019", "\\u2019"), html);
    }

    [Fact]
    public async Task Every_job_and_every_bullet_is_on_the_page()
    {
        Assert.Equal(7, Resume.Roles.Count);
        Assert.Equal(32, Resume.Roles.Sum(r => r.Bullets.Count));
        var html = await HomeAsync();
        foreach (var role in Resume.Roles)
        {
            Assert.Contains(role.Title, html);
            foreach (var bullet in role.Bullets)
                Assert.Contains(bullet.Value, WebUtility.HtmlDecode(html));
        }
    }

    [Fact]
    public async Task Never_says_backend()
    {
        Assert.DoesNotContain("backend", await HomeAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Never_shows_a_phone_number_or_an_excluded_project()
    {
        var html = await HomeAsync();
        Assert.DoesNotMatch(PhoneNumber(), html);
        foreach (var excluded in new[] { "interview helper", "AutoApply", "CoverLetter", "cover letter" })
            Assert.DoesNotContain(excluded, html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Links_only_to_public_github_pages_never_to_private_repos()
    {
        var html = await HomeAsync();
        var allowed = new[] { Profile.GitHub, Profile.SiteRepo };
        foreach (Match m in GitHubLink().Matches(html))
            Assert.Contains(m.Groups[1].Value.TrimEnd('/'), allowed);
    }

    [Fact]
    public async Task Ids_are_unique_and_every_anchor_lands()
    {
        var html = await HomeAsync();
        var ids = IdAttribute().Matches(html).Select(m => m.Groups[1].Value).ToList();
        Assert.Equal(ids.Count, ids.Distinct().Count());
        foreach (Match m in HashLink().Matches(html))
            Assert.Contains(m.Groups[1].Value, ids);
    }

    [Fact]
    public async Task Every_image_has_alt_text_and_dimensions()
    {
        var html = await HomeAsync();
        var images = ImgTag().Matches(html).Select(m => m.Value).Where(t => !t.Contains("id=\"viewer-img\"")).ToList();
        Assert.Equal(10, images.Count);
        Assert.All(images, img =>
        {
            Assert.Matches("alt=\"[^\"]{10,}\"", img);
            Assert.Matches("width=\"\\d+\"", img);
            Assert.Matches("height=\"\\d+\"", img);
        });
    }

    [Fact]
    public async Task No_status_light_pretends_to_be_live_data()
    {
        Assert.DoesNotContain("checked 2 minutes ago", await HomeAsync());
    }

    [Theory]
    [InlineData("/scott-bush-resume.pdf", "application/pdf")]
    [InlineData("/og.png", "image/png")]
    [InlineData("/fonts/archivo-latin.woff2", "font/woff2")]
    public async Task Linked_files_exist(string path, string contentType)
    {
        var response = await _client.GetAsync(path);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(contentType, response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Healthz_reports_the_build()
    {
        var body = await _client.GetStringAsync("/healthz");
        Assert.Contains("\"status\":\"ok\"", body);
        Assert.Contains("\"commit\":", body);
    }

    [Fact]
    public async Task Www_redirects_permanently_to_the_bare_domain()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/some/path?x=1");
        request.Headers.Host = "www.scottbush.dev";
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);
        Assert.Equal("https://scottbush.dev/some/path?x=1", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Unknown_paths_get_a_real_404_page()
    {
        var response = await _client.GetAsync("/nothing-here");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Nothing lives at this address.", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Head_requests_succeed_for_link_previews()
    {
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, "/"));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Pages_cannot_be_framed()
    {
        var response = await _client.GetAsync("/");
        Assert.Contains("frame-ancestors 'none'", response.Headers.GetValues("Content-Security-Policy").Single());
    }

    [GeneratedRegex(@"\(?\b\d{3}\)?[ .-]\d{3}[ .-]\d{4}\b")] private static partial Regex PhoneNumber();
    [GeneratedRegex("href=\"(https://github\\.com/[^\"]*)\"")] private static partial Regex GitHubLink();
    [GeneratedRegex(" id=\"([^\"]+)\"")] private static partial Regex IdAttribute();
    [GeneratedRegex("href=\"#([^\"]+)\"")] private static partial Regex HashLink();
    [GeneratedRegex("<img [^>]+>")] private static partial Regex ImgTag();
}
