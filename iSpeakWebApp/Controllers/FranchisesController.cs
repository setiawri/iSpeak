using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;
using System.Web;

namespace iSpeakWebApp.Controllers
{
    /*
     * Franchises is controlled by central.
     */

    public class FranchisesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: Franchises
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Franchises_View)
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

        // POST: Franchises
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active)
        {
            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get(FILTER_Keyword, FILTER_Active));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: Franchises/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Franchises_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(new FranchisesModel());
        }

        // POST: Franchises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FranchisesModel model, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Name))
                    ModelState.AddModelError(FranchisesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    model.Active = true;
                    db.Franchises.Add(model);
                    db.SaveChanges();
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: Franchises/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Franchises_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get((Guid)id));
        }

        // POST: Franchises/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FranchisesModel modifiedModel, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(FranchisesModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    FranchisesModel originalModel = db.Franchises.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, FranchisesModel.COL_Name.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, FranchisesModel.COL_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, FranchisesModel.COL_Active.LogDisplay);

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
            controller.ViewBag.Franchises = new SelectList(get(), FranchisesModel.COL_Id.Name, FranchisesModel.COL_Name.Name);
        }

        public static SelectList getDropDownListSelectList(Guid selectedValue)
        {
            return new SelectList(get(), FranchisesModel.COL_Id.Name, FranchisesModel.COL_Name.Name, selectedValue);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Name)
        {
            return db.Database.SqlQuery<FranchisesModel>(@"
                        SELECT Franchises.*
                        FROM Franchises
                        WHERE 1=1 
							AND (@Id IS NOT NULL OR Franchises.Name = @Name)
							AND (@Id IS NULL OR (Franchises.Name = @Name AND Franchises.Id <> @Id))
                    ",
                    DBConnection.getSqlParameter(FranchisesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(FranchisesModel.COL_Name.Name, Name)
                ).Count() > 0;
        }

        public static List<FranchisesModel> get(int? FILTER_Active, string IdList) { return get(null, FILTER_Active, null, IdList); }
        public List<FranchisesModel> get(string FILTER_Keyword, int? FILTER_Active) { return get(null, FILTER_Active, FILTER_Keyword, null); }
        public FranchisesModel get(Guid Id) { return get(Id, null, null, null).FirstOrDefault(); }
        public static List<FranchisesModel> get() { return get(null, null, null, null); }
        public static List<FranchisesModel> get(Guid? Id, int? FILTER_Active, string FILTER_Keyword, string IdList)
        {
            string IdListClause = "";
            if (!string.IsNullOrEmpty(IdList))
                IdListClause = string.Format(" AND Franchises.Id IN ({0}) ", LIBWebMVC.UtilWebMVC.convertToSqlIdList(IdList));

            string sql = string.Format(@"
                        SELECT Franchises.*
                        FROM Franchises
                        WHERE 1=1
							AND (@Id IS NULL OR Franchises.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Active IS NULL OR Franchises.Active = @Active)
    							AND (@FILTER_Keyword IS NULL OR (Franchises.Name LIKE '%'+@FILTER_Keyword+'%'))
                                {0}
                            ))
						ORDER BY Franchises.Name ASC
                    ", IdListClause);

            return new DBContext().Database.SqlQuery<FranchisesModel>(sql,
                    DBConnection.getSqlParameter(FranchisesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(FranchisesModel.COL_Active.Name, FILTER_Active),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                    DBConnection.getSqlParameter("IdList", IdList)
                ).ToList();
        }

        /******************************************************************************************************************************************************/
    }
}