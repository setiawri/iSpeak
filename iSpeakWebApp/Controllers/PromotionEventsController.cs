using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class PromotionEventsController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: PromotionEvents
        public ActionResult Index(int? rss)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            return View(db.PromotionEvents);
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PromotionEvents/Create
        public ActionResult Create()
        {
            return View(new PromotionEventsModel());
        }

        // POST: PromotionEvents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PromotionEventsModel model)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Create, null, model.Name))
                    ModelState.AddModelError(PromotionEventsModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    model.Branches_Id = Helper.getActiveBranchId(Session);
                    db.PromotionEvents.Add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: PromotionEvents/Edit/{id}
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            return View(db.PromotionEvents.Find(id));
        }

        // POST: PromotionEvents/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PromotionEventsModel modifiedModel)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Edit, modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(PromotionEventsModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    PromotionEventsModel originalModel = db.PromotionEvents.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Helper.append<BranchesModel>(log, originalModel.Branches_Id, modifiedModel.Branches_Id, PromotionEventsModel.COL_Branches_Id.LogDisplay);
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, PromotionEventsModel.COL_Name.LogDisplay);
                    log = Helper.append(log, originalModel.Location, modifiedModel.Location, PromotionEventsModel.COL_Location.LogDisplay);
                    log = Helper.append(log, originalModel.TotalDays, modifiedModel.TotalDays, PromotionEventsModel.COL_TotalDays.LogDisplay);
                    log = Helper.append(log, originalModel.EventFee, modifiedModel.EventFee, PromotionEventsModel.COL_EventFee.LogDisplay);
                    log = Helper.append(log, originalModel.PersonnelCost, modifiedModel.PersonnelCost, PromotionEventsModel.COL_PersonnelCost.LogDisplay);
                    log = Helper.append(log, originalModel.AdditionalCost, modifiedModel.AdditionalCost, PromotionEventsModel.COL_AdditionalCost.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, PromotionEventsModel.COL_Notes.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                    {
                        db.Entry(modifiedModel).State = EntityState.Modified;
                        ActivityLogsController.AddEditLog(db, Session, modifiedModel.Id, log);
                        db.SaveChanges();
                    }

                    return RedirectToAction(nameof(Index));
                }
            }

            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public static List<PromotionEventsModel> get()
        {
            return new DBContext().PromotionEvents.AsNoTracking()
                .OrderBy(x => x.Name)
                .ToList();
        }

        public bool isExists(EnumActions action, Guid? id, object value)
        {
            var result = action == EnumActions.Create
                ? db.PromotionEvents.AsNoTracking().Where(x => x.Name.ToLower() == value.ToString().ToLower()).FirstOrDefault()
                : db.PromotionEvents.AsNoTracking().Where(x => x.Name.ToLower() == value.ToString().ToLower() && x.Id != id).FirstOrDefault();
            return result != null;
        }

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller)
        {
            controller.ViewBag.PromotionEvents = new SelectList(db.PromotionEvents.AsNoTracking().OrderBy(x => x.Name).ToList(), PromotionEventsModel.COL_Id.Name, PromotionEventsModel.COL_Name.Name);
        }

        /******************************************************************************************************************************************************/
    }
}