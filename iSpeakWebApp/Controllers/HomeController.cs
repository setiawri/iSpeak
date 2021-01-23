using System.Web.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using iSpeakWebApp.Models;

namespace iSpeakWebApp.Controllers
{
    public class HomeController : Controller
    {
        private static readonly DBContext db = new DBContext();

        /* INDEX PAGE *****************************************************************************************************************************************/

        public ActionResult Index()
        {
            Guid BranchId = Helper.getBranchId(Session);
            List<RemindersModel> remindersModel = db.Reminders.AsNoTracking()
                .Where(x => x.Branches_Id == BranchId
                    && x.Status_enumid != EnumReminderStatuses.Completed 
                    && x.Status_enumid != EnumReminderStatuses.Cancel).ToList();

            ViewBag.ActiveReminderCount = remindersModel.Count;
            return View(remindersModel);
        }

        /* METHODS ********************************************************************************************************************************************/


        /******************************************************************************************************************************************************/
    }
}