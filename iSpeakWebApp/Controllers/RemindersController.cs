using iSpeakWebApp.Models;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using LIBUtil;

using System.Web.Mvc;

namespace iSpeakWebApp.Controllers
{
    public class RemindersController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX PARTIAL **************************************************************************************************************************************/

        public PartialViewResult IndexPartial(int? rss)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            var models = getReminders(Helper.getActiveBranchId(Session), null);
            ViewBag.ReminderCount = models.Count;

            return PartialView("IndexPartial", models);
        }

        /* CREATE *********************************************************************************************************************************************/

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RemindersModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                model.Branches_Id = Helper.getActiveBranchId(Session);
                db.Reminders.Add(model);
                ActivityLogsController.AddCreateLog(db, Session, model.Id);
                db.SaveChanges();
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        public ActionResult Edit(Guid? Id)
        {
            if (Id == null)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            RemindersModel model = db.Reminders.Where(x => x.Id == Id).FirstOrDefault();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RemindersModel model, string Notes)
        {
            if (ModelState.IsValid)
            {
                string log = string.Format("[{0}] {1}", Util.GetEnumDescription<EnumReminderStatuses>(model.Status_enumid), Notes);

                db.Entry(model).State = EntityState.Modified;
                ActivityLogsController.Add(db, Session, model.Id, log);
                db.SaveChanges();

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return View(model);
        }

        /* METHODS ********************************************************************************************************************************************/

        public JsonResult GetData(int? status)
        {
            EnumReminderStatuses? reminderStatus;
            if (status == null)
                reminderStatus = null;
            else
                reminderStatus = Util.parseEnum<EnumReminderStatuses>(status);

            List<RemindersModel> models = new RemindersController().getReminders(Helper.getActiveBranchId(Session), reminderStatus);

            return Json(new { result = models, count = models.Count }, JsonRequestBehavior.AllowGet);
        }

        public List<RemindersModel> getReminders(Guid Branches_Id, EnumReminderStatuses? status) { return get(Branches_Id, null, status); }
        public List<RemindersModel> get(Guid? Branches_Id, Guid? Id, EnumReminderStatuses? status)
        {
            return new DBContext().Reminders.AsNoTracking()
                .Where(x => x.Branches_Id == Branches_Id
                    && (status == null || x.Status_enumid == status)
                    && (status != null || (
                        x.Status_enumid != EnumReminderStatuses.Completed
                        && x.Status_enumid != EnumReminderStatuses.Cancel
                    ))
                ).ToList();
        }

        /******************************************************************************************************************************************************/
    }
}