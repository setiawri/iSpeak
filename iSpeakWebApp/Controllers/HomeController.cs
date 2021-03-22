using System.Web.Mvc;

namespace iSpeakWebApp.Controllers
{
    public class HomeController : Controller
    {
        private static readonly DBContext db = new DBContext();

        /* INDEX PAGE *****************************************************************************************************************************************/

        public ActionResult Index(int? rss)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            return View();
        }

        /* METHODS ********************************************************************************************************************************************/

        /******************************************************************************************************************************************************/
    }
}