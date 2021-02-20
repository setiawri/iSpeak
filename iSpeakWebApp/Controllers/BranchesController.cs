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
    public static class BranchSelectLists
    {
        static SelectList BranchList;

        public static SelectList get()
        {
            if (BranchList == null)
                update();

            return BranchList;
        }

        //IMPROVEMENT: need to call this method when branches table change
        public static void update()
        {
            DBContext db = new DBContext();
            BranchList = new SelectList(BranchesController.get(true).Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name.ToString() }), "Value", "Text");
        }
    }

    public class BranchesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: Branches
        public ActionResult Index(int? rss)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            return View(db.Branches);
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: Branches/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Branches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BranchesModel model)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Create, null, model.Name))
                    ModelState.AddModelError(BranchesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    model.Active = true;
                    db.Branches.Add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: Branches/Edit/{id}
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            return View(db.Branches.Find(id));
        }

        // POST: Branches/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BranchesModel modifiedModel)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Edit, modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(BranchesModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    BranchesModel originalModel = db.Branches.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Util.webAppendChange(log, originalModel.Name, modifiedModel.Name, ActivityLogsController.editStringFormat(BranchesModel.COL_Name.LogDisplay));
                    log = Util.webAppendChange(log, originalModel.Address, modifiedModel.Address, ActivityLogsController.editStringFormat(BranchesModel.COL_Address.LogDisplay));
                    log = Util.webAppendChange(log, originalModel.PhoneNumber, modifiedModel.PhoneNumber, ActivityLogsController.editStringFormat(BranchesModel.COL_PhoneNumber.LogDisplay));
                    log = Util.webAppendChange(log, originalModel.Notes, modifiedModel.Notes, ActivityLogsController.editStringFormat(BranchesModel.COL_Notes.LogDisplay));
                    log = Util.webAppendChange(log, originalModel.InvoiceHeaderText, modifiedModel.InvoiceHeaderText, ActivityLogsController.editStringFormat(BranchesModel.COL_InvoiceHeaderText.LogDisplay));
                    log = Util.webAppendChange(log, originalModel.Active, modifiedModel.Active, ActivityLogsController.editStringFormat(BranchesModel.COL_Active.LogDisplay));

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

        public static List<BranchesModel> get(bool isActiveOnly)
        {
            return new DBContext().Branches.AsNoTracking()
                .Where(x => x.Active == isActiveOnly)
                .OrderBy(x => x.Name)
                .ToList();
        }

        public bool isExists(EnumActions action, Guid? id, object value)
        {
            var result = action == EnumActions.Create
                ? db.Branches.AsNoTracking().Where(x => x.Name.ToLower() == value.ToString().ToLower()).FirstOrDefault()
                : db.Branches.AsNoTracking().Where(x => x.Name.ToLower() == value.ToString().ToLower() && x.Id != id).FirstOrDefault();
            return result != null;
        }

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller)
        {
            controller.ViewBag.Branches = new SelectList(db.Branches.AsNoTracking().OrderBy(x => x.Name).ToList(), BranchesModel.COL_Id.Name, BranchesModel.COL_Name.Name);
        }

        /******************************************************************************************************************************************************/
    }
}