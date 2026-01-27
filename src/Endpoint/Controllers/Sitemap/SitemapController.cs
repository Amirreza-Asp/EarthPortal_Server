using System.Globalization;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Endpoint.Controllers.Sitemap;

[ApiController]
[Route("api/sitemap.xml")]
[Route("sitemap.xml")]
public class SitemapController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly string _baseUrl = "https://zamin.gov.ir";

    public SitemapController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var urls = new List<SitemapUrl>();

        urls.Add(
            new SitemapUrl(
                Location: $"{_baseUrl}/",
                LastModofied: DateTime.UtcNow,
                ChangeFrequency: "daily",
                Priority: 1.0
            )
        );

        urls.Add(
            new SitemapUrl(
                Location: $"{_baseUrl}/news",
                LastModofied: DateTime.UtcNow,
                ChangeFrequency: "daily",
                Priority: 0.9
            )
        );

        urls.Add(
            new SitemapUrl(
                Location: $"{_baseUrl}/law",
                LastModofied: DateTime.UtcNow,
                ChangeFrequency: "daily",
                Priority: 0.8
            )
        );

        urls.Add(
            new SitemapUrl(
                Location: $"{_baseUrl}/multimedia",
                LastModofied: DateTime.UtcNow,
                ChangeFrequency: "daily",
                Priority: 0.8
            )
        );

        var news = await _dbContext
            .News.OrderByDescending(n => n.DateOfRegisration)
            .Select(n => new { n.ShortLink, n.DateOfRegisration })
            .ToListAsync();

        foreach (var item in news)
        {
            urls.Add(
                new SitemapUrl(
                    $"{_baseUrl}/news/{item.ShortLink}",
                    item.DateOfRegisration,
                    "weekly",
                    0.7
                )
            );
        }

        var xml = BuildSiteMap(urls);
        return Content(xml, "application/xml");
    }

    private static string BuildSiteMap(IEnumerable<SitemapUrl> urls)
    {
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

        var document = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement(
                ns + "urlset",
                urls.Select(u => new XElement(
                    ns + "url",
                    new XElement(ns + "loc", u.Location),
                    new XElement(ns + "lastmod", u.LastModofied.ToString("yyyy-MM-dd")),
                    new XElement(ns + "changefreq", u.ChangeFrequency),
                    new XElement(
                        ns + "priority",
                        u.Priority.ToString("0.0", CultureInfo.InvariantCulture)
                    )
                ))
            )
        );

        return document.ToString();
    }

    private record SitemapUrl(
        string Location,
        DateTime LastModofied,
        string ChangeFrequency,
        double Priority
    );
}
