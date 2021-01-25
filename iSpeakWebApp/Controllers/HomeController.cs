using System.Web.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity;
using iSpeakWebApp.Models;
using LIBUtil;

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