using EPiServer.Web.Mvc;
using Lorem.Models.Pages;
using Lorem.Models.ViewModels;
using System.Web.Mvc;

namespace Lorem.Controllers
{
    public class StartPageController
        : PageController<StartPage>
    {
        public ActionResult Index(StartPage currentPage)
        {
            return View(new DefaultPageViewModel<StartPage>(currentPage));
        }
    }
}