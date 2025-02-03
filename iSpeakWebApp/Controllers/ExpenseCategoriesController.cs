using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    /*
     * ExpenseCategories is filtered by Franchise. 
     */

    public class ExpenseCategoriesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: ExpenseCategories
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).ExpenseCategories_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                setViewBag(FILTER_Keyword, FILTER_Active);
                return View(get(FILTER_Keyword, FILTER_Active));
            }
        }

        // POST: ExpenseCategories
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active)
        {
            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get(FILTER_Keyword, FILTER_Active));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: ExpenseCategories/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).ExpenseCategories_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(new ExpenseCategoriesModel());
        }

        // POST: ExpenseCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ExpenseCategoriesModel model, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Name))
                    ModelState.AddModelError(ExpenseCategoriesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    model.Active = true;
                    db.ExpenseCategories.Add(model);
                    db.SaveChanges();
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: ExpenseCategories/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).ExpenseCategories_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get((Guid)id));
        }

        // POST: ExpenseCategories/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ExpenseCategoriesModel modifiedModel, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(ExpenseCategoriesModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    ExpenseCategoriesModel originalModel = db.ExpenseCategories.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, ExpenseCategoriesModel.COL_Name.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, ExpenseCategoriesModel.COL_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, ExpenseCategoriesModel.COL_Active.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                    {
                        db.Entry(modifiedModel).State = EntityState.Modified;
                        db.SaveChanges();
                        ActivityLogsController.AddEditLog(db, Session, modifiedModel.Id, log);
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
            controller.ViewBag.ExpenseCategories = new SelectList(get(), ExpenseCategoriesModel.COL_Id.Name, ExpenseCategoriesModel.COL_Name.Name);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Name)
        {
            return db.Database.SqlQuery<ExpenseCategoriesModel>(@"
                        SELECT ExpenseCategories.*
                        FROM ExpenseCategories
                        WHERE 1=1 
							AND (@Id IS NOT NULL OR ExpenseCategories.Name = @Name)
							AND (@Id IS NULL OR (ExpenseCategories.Name = @Name AND ExpenseCategories.Id <> @Id))
                    ",
                    DBConnection.getSqlParameter(ExpenseCategoriesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(ExpenseCategoriesModel.COL_Name.Name, Name)
                ).Count() > 0;
        }

        public List<ExpenseCategoriesModel> get(string FILTER_Keyword, int? FILTER_Active) { return get(null, FILTER_Active, FILTER_Keyword); }
        public ExpenseCategoriesModel get(Guid Id) { return get(Id, null, null).FirstOrDefault(); }
        public static List<ExpenseCategoriesModel> get() { return get(null, null, null); }
        public static List<ExpenseCategoriesModel> get(Guid? Id, int? FILTER_Active, string FILTER_Keyword)
        {
            return new DBContext().Database.SqlQuery<ExpenseCategoriesModel>(@"
                        SELECT ExpenseCategories.*
                        FROM ExpenseCategories
                        WHERE 1=1
							AND (@Id IS NULL OR ExpenseCategories.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Active IS NULL OR ExpenseCategories.Active = @Active)
    							AND (@FILTER_Keyword IS NULL OR (ExpenseCategories.Name LIKE '%'+@FILTER_Keyword+'%'))
                            ))
						ORDER BY ExpenseCategories.Name ASC
                    ",
                    DBConnection.getSqlParameter(ExpenseCategoriesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(ExpenseCategoriesModel.COL_Active.Name, FILTER_Active),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
                ).ToList();
        }

        /******************************************************************************************************************************************************/
    }
}