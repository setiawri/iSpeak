using System.Web.Mvc;
using System;
using System.Linq;
using iSpeakWebApp.Models;

namespace iSpeakWebApp.Controllers
{
    public class HomeController : Controller
    {
        private static readonly DBContext db = new DBContext();

        public ActionResult Index()
        {

            return View();
        }
    }
}