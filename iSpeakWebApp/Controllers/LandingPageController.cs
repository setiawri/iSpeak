using System.Web.Mvc;

namespace iSpeakWebApp.Controllers
{
    public class LandingPageController : Controller
    {
        public ActionResult LandingPage()
        {
            return View();
        }
    }
}