using EPiServer.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using TestSite.Models.Pages;

namespace TestSite.Controllers.Pages;

public class HomePageController : PageController<HomePage>
{
    public virtual IActionResult Index(HomePage currentContent)
    {
        return View("~/Views/Pages/Homepage.cshtml", currentContent);
    }
}
