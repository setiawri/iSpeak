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

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: PettyCashRecordsCategories
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).PettyCashRecordsCategories_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                return View(get(FILTER_Keyword, FILTER_Active));
            }
        }

        // POST: PettyCashRecordsCategories
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active)
        {
            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get(FILTER_Keyword, FILTER_Active));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PettyCashRecordsCategories/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).PettyCashRecordsCategories_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View();
        }

        // POST: PettyCashRecordsCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PettyCashRecordsCategoriesModel model, string FILTER_Keyword, int? FILTER_Active)
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
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: PettyCashRecordsCategories/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).PettyCashRecordsCategories_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get((Guid)id));
        }

        // POST: PettyCashRecordsCategories/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PettyCashRecordsCategoriesModel modifiedModel, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(PettyCashRecordsCategoriesModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    PettyCashRecordsCategoriesModel originalModel = db.PettyCashRecordsCategories.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, PettyCashRecordsCategoriesModel.COL_Name.LogDisplay);
                    log = Helper.append(log, originalModel.Default_row, modifiedModel.Default_row, PettyCashRecordsCategoriesModel.COL_Default_row.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, PettyCashRecordsCategoriesModel.COL_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, PettyCashRecordsCategoriesModel.COL_Active.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                    {
                        db.Entry(modifiedModel).State = EntityState.Modified;
                        ActivityLogsController.AddEditLog(db, Session, modifiedModel.Id, log);
                        db.SaveChanges();
                    }

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public static void setDropDownListViewBag(Controller controller)
        {
            controller.ViewBag.PettyCashRecordsCategories = new SelectList(get(), PettyCashRecordsCategoriesModel.COL_Id.Name, PettyCashRecordsCategoriesModel.COL_Name.Name);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Name)
        {
            return db.Database.SqlQuery<PettyCashRecordsCategoriesModel>(@"
                        SELECT PettyCashRecordsCategories.*
                        FROM PettyCashRecordsCategories
                        WHERE 1=1 
							AND (@Id IS NOT NULL OR PettyCashRecordsCategories.Name = @Name)
							AND (@Id IS NULL OR (PettyCashRecordsCategories.Name = @Name AND PettyCashRecordsCategories.Id <> @Id))
                    ",
                    DBConnection.getSqlParameter(PettyCashRecordsCategoriesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(PettyCashRecordsCategoriesModel.COL_Name.Name, Name)
                ).Count() > 0;
        }

        public List<PettyCashRecordsCategoriesModel> get(string FILTER_Keyword, int? FILTER_Active) { return get(null, FILTER_Active, FILTER_Keyword); }
        public PettyCashRecordsCategoriesModel get(Guid Id) { return get(Id, null, null).FirstOrDefault(); }
        public static List<PettyCashRecordsCategoriesModel> get() { return get(null, null, null); }
        public static List<PettyCashRecordsCategoriesModel> get(Guid? Id, int? FILTER_Active, string FILTER_Keyword)
        {
            return new DBContext().Database.SqlQuery<PettyCashRecordsCategoriesModel>(@"
                        SELECT PettyCashRecordsCategories.*
                        FROM PettyCashRecordsCategories
                        WHERE 1=1
							AND (@Id IS NULL OR PettyCashRecordsCategories.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Active IS NULL OR PettyCashRecordsCategories.Active = @Active)
    							AND (@FILTER_Keyword IS NULL OR (PettyCashRecordsCategories.Name LIKE '%'+@FILTER_Keyword+'%'))
                            ))
						ORDER BY PettyCashRecordsCategories.Name ASC
                    ",
                    DBConnection.getSqlParameter(PettyCashRecordsCategoriesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(PettyCashRecordsCategoriesModel.COL_Active.Name, FILTER_Active),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
                ).ToList();
        }

        /******************************************************************************************************************************************************/
    }
}