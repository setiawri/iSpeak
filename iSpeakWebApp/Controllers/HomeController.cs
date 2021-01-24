using System.Web.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class HomeController : Controller
    {
        private static readonly DBContext db = new DBContext();

        /* INDEX PAGE *****************************************************************************************************************************************/

        public ActionResult Index()
        {
            return View();
        }

        /* REMINDER PARTIAL VIEW ******************************************************************************************************************************/

        public PartialViewResult Reminder()
        {
            Guid BranchId = Helper.getActiveBranchId(Session);
            List<RemindersModel> models = db.Reminders.AsNoTracking()
                .Where(x => x.Branches_Id == BranchId
                    && x.Status_enumid != EnumReminderStatuses.Completed
                    && x.Status_enumid != EnumReminderStatuses.Cancel).ToList();

            //ViewBag.ActiveReminderCount = remindersModels.Count;

            return PartialView("Reminder", models);
        }

        /* BIRTHDAY PARTIAL VIEW ******************************************************************************************************************************/

        public PartialViewResult Birthday()
        {
            Guid BranchId = Helper.getActiveBranchId(Session);

            DateTime startDate = (DateTime)Util.getFirstDayOfSelectedMonth(DateTime.Now);
            DateTime endDate = (DateTime)Util.getLastDayOfSelectedMonth(DateTime.Now);
            List<UserAccountsModel> models = db.UserAccounts.AsNoTracking()
                .Where(x => x.Branches_Id == BranchId
                    && x.Birthday.Month == DateTime.Now.Month
                    && x.Birthday.Day >= DateTime.Now.Day
                    && x.Active == true)
                .OrderBy(x => x.Birthday.Day)
                .ToList();

            //ViewBag.ActiveReminderCount = remindersModels.Count;
            UserAccountRolesController.setDropDownListViewBag(db, this);

            return PartialView("Birthday", models);
        }

        public JsonResult UpdateBirthday(int? month, Guid? UserAccountRoles_Id)
        {
            List <UserAccountsModel> models = new UserAccountsController().getBirthdays(Helper.getActiveBranchId(Session), UserAccountRoles_Id, month);

            //ViewBag.ActiveReminderCount = remindersModels.Count;

            return Json(new { result = models, count = models.Count }, JsonRequestBehavior.AllowGet);
        }

        /* METHODS ********************************************************************************************************************************************/

        /******************************************************************************************************************************************************/
    }
}