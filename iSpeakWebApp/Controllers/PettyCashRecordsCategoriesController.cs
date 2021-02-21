using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using iSpeakWebApp.Models;

namespace iSpeakWebApp.Controllers
{
    public class PettyCashRecordsCategoriesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: PettyCashRecordsCategories
        public ActionResult Index(int? rss)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            return View(db.PettyCashRecordsCategories);
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PettyCashRecordsCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PettyCashRecordsCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PettyCashRecordsCategoriesModel model)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Create, null, model.Name))
                    ModelState.AddModelError(PettyCashRecordsCategoriesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    model.Active = true;
                    db.PettyCashRecordsCategories.Add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: PettyCashRecordsCategories/Edit/{id}
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            return View(db.PettyCashRecordsCategories.Find(id));
        }

        // POST: PettyCashRecordsCategories/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PettyCashRecordsCategoriesModel modifiedModel)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Edit, modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(PettyCashRecordsCategoriesModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    PettyCashRecordsCategoriesModel originalModel = db.PettyCashRecordsCategories.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Helper.appendLog(log, originalModel.Name, modifiedModel.Name, PettyCashRecordsCategoriesModel.COL_Name.LogDisplay);
                    log = Helper.appendLog(log, originalModel.Default_row, modifiedModel.Default_row, PettyCashRecordsCategoriesModel.COL_Default_row.LogDisplay);
                    log = Helper.appendLog(log, originalModel.Notes, modifiedModel.Notes, PettyCashRecordsCategoriesModel.COL_Notes.LogDisplay);
                    log = Helper.appendLog(log, originalModel.Active, modifiedModel.Active, PettyCashRecordsCategoriesModel.COL_Active.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                    {
                        db.PettyCashRecordsCategories.

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

        public bool isExists(EnumActions action, Guid? id, object value)
        {
            var result = action == EnumActions.Create
                ? db.PettyCashRecordsCategories.AsNoTracking().Where(x => x.Name.ToLower() == value.ToString().ToLower()).FirstOrDefault()
                : db.PettyCashRecordsCategories.AsNoTracking().Where(x => x.Name.ToLower() == value.ToString().ToLower() && x.Id != id).FirstOrDefault();
            return result != null;
        }

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller)
        {
            controller.ViewBag.PettyCashRecordsCategories = new SelectList(db.PettyCashRecordsCategories.AsNoTracking().OrderBy(x => x.Name).ToList(), PettyCashRecordsCategoriesModel.COL_Id.Name, PettyCashRecordsCategoriesModel.COL_Name.Name);
        }

        /******************************************************************************************************************************************************/
    }
}