using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class PettyCashRecordsCategoriesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: PettyCashRecordsCategories
        public ActionResult Index(int? rss, int? Active)
        {
            ViewBag.RemoveDatatablesStateSave = rss;
            ViewBag.Active = Active;

            return View(get(null, Active));
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
                if (isExists(null, model.Name))
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
                if (isExists(modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(PettyCashRecordsCategoriesModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    PettyCashRecordsCategoriesModel originalModel = db.PettyCashRecordsCategories.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, ActivityLogsController.editStringFormat(PettyCashRecordsCategoriesModel.COL_Name.LogDisplay));
                    log = Helper.append(log, originalModel.Default_row, modifiedModel.Default_row, ActivityLogsController.editStringFormat(PettyCashRecordsCategoriesModel.COL_Default_row.LogDisplay));
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, ActivityLogsController.editStringFormat(PettyCashRecordsCategoriesModel.COL_Notes.LogDisplay));
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, ActivityLogsController.editStringFormat(PettyCashRecordsCategoriesModel.COL_Active.LogDisplay));

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

        public bool isExists(Guid? id, object value)
        {
            return get(id, null).Count > 0;
        }

        public List<PettyCashRecordsCategoriesModel> get(Guid? Id, int? Active)
        {
            List<PettyCashRecordsCategoriesModel> models = db.Database.SqlQuery<PettyCashRecordsCategoriesModel>(@"
                        SELECT PettyCashRecordsCategories.*
                        FROM PettyCashRecordsCategories
                        WHERE 1=1
							AND (@Id IS NULL OR PettyCashRecordsCategories.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Active IS NULL OR PettyCashRecordsCategories.Active = @Active)
                            ))
						ORDER BY PettyCashRecordsCategories.Name ASC
                    ",
                    DBConnection.getSqlParameter(PettyCashRecordsCategoriesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(PettyCashRecordsCategoriesModel.COL_Active.Name, Active)
                ).ToList();

            return models;
        }

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller)
        {
            controller.ViewBag.PettyCashRecordsCategories = new SelectList(db.PettyCashRecordsCategories.AsNoTracking().OrderBy(x => x.Name).ToList(), PettyCashRecordsCategoriesModel.COL_Id.Name, PettyCashRecordsCategoriesModel.COL_Name.Name);
        }

        /******************************************************************************************************************************************************/
    }
}