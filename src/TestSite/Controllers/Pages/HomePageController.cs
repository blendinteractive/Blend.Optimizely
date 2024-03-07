using Blend.Optimizely;
using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TestSite.Models.Pages;

namespace TestSite.Controllers.Pages;

public class HomePageController : PageController<HomePage>
{
    public virtual IActionResult Index(HomePage currentContent)
    {
        var links = currentContent.Links.HasValue() ? currentContent.Links.Select(x => x.ResolveUrl()!).ToList() : Enumerable.Empty<string>();

        return View("~/Views/Pages/Homepage.cshtml", currentContent);
    }
}
