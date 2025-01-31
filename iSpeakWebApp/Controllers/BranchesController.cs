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
    public class BranchesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
            FranchisesController.setDropDownListViewBag(this);
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: Branches
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Branches_View)
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

        // POST: Branches
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active)
        {
            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get(FILTER_Keyword, FILTER_Active));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: Branches/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Branches_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(new BranchesModel());
        }

        // POST: Branches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BranchesModel model, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Name))
                    ModelState.AddModelError(BranchesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: Branches/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Branches_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get((Guid)id));
        }

        // POST: Branches/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BranchesModel modifiedModel, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(BranchesModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    BranchesModel originalModel = get(modifiedModel.Id);

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, BranchesModel.COL_Name.LogDisplay);
                    log = Helper.append(log, originalModel.Franchises_Id, modifiedModel.Franchises_Id, BranchesModel.COL_Franchises_Id.LogDisplay);
                    log = Helper.append(log, originalModel.Address, modifiedModel.Address, BranchesModel.COL_Address.LogDisplay);
                    log = Helper.append(log, originalModel.PhoneNumber, modifiedModel.PhoneNumber, BranchesModel.COL_PhoneNumber.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, BranchesModel.COL_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.InvoiceHeaderText, modifiedModel.InvoiceHeaderText, BranchesModel.COL_InvoiceHeaderText.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, BranchesModel.COL_Active.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                    {
                        update(modifiedModel, log);
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
            controller.ViewBag.Branches = new SelectList(get(Helper.getUserFranchiseIdForQuery(controller.Session)), BranchesModel.COL_Id.Name, BranchesModel.COL_Name.Name);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Name)
        {
            return db.Database.SqlQuery<BranchesModel>(@"
                        SELECT Branches.*
                        FROM Branches
                        WHERE 1=1 
							AND (@Id IS NOT NULL OR Branches.Name = @Name)
							AND (@Id IS NULL OR (Branches.Name = @Name AND Branches.Id <> @Id))
                    ",
                    DBConnection.getSqlParameter(BranchesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(BranchesModel.COL_Name.Name, Name)
                ).Count() > 0;
        }

        public static List<BranchesModel> get(int? FILTER_Active, string IdList) { return get(null, FILTER_Active, null, IdList, null); }
        public List<BranchesModel> get(string FILTER_Keyword, int? FILTER_Active) { return get(null, FILTER_Active, FILTER_Keyword, null, null); }
        public BranchesModel get(Guid Id) { return get(Id, null, null, null, null).FirstOrDefault(); }
        public static List<BranchesModel> get(Guid? Franchises_Id) { return get(null, null, null, null, Franchises_Id); }
        public static List<BranchesModel> get(Guid? Id, int? FILTER_Active, string FILTER_Keyword, string IdList, Guid? Franchises_Id)
        {
            string IdListClause = "";
            if (!string.IsNullOrEmpty(IdList))
                IdListClause = string.Format(" AND Branches.Id IN ({0}) ", LIBWebMVC.UtilWebMVC.convertToSqlIdList(IdList));

            string sql = string.Format(@"
                        SELECT Branches.*,
                            Franchises.Name AS Franchises_Name
                        FROM Branches
                            LEFT JOIN Franchises ON Franchises.Id = Branches.Franchises_Id
                        WHERE 1=1
							AND (@Id IS NULL OR Branches.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Active IS NULL OR Branches.Active = @Active)
    							AND (@FILTER_Keyword IS NULL OR (Branches.Name LIKE '%'+@FILTER_Keyword+'%'))
                                AND (@Franchises_Id IS NULL OR Branches.Franchises_Id = @Franchises_Id)
                                {0}
                            ))
						ORDER BY Branches.Name ASC
                    ", IdListClause);

            return new DBContext().Database.SqlQuery<BranchesModel>(sql,
                    DBConnection.getSqlParameter(BranchesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(BranchesModel.COL_Active.Name, FILTER_Active),
                    DBConnection.getSqlParameter(BranchesModel.COL_Franchises_Id.Name, Franchises_Id),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                    DBConnection.getSqlParameter("IdList", IdList)
                ).ToList();
        }

        public void add(BranchesModel model)
        {
            LIBWebMVC.WebDBConnection.Insert(db.Database, "Branches",
                DBConnection.getSqlParameter(BranchesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(BranchesModel.COL_Name.Name, model.Name),
                DBConnection.getSqlParameter(BranchesModel.COL_Franchises_Id.Name, model.Franchises_Id),
                DBConnection.getSqlParameter(BranchesModel.COL_Address.Name, model.Address),
                DBConnection.getSqlParameter(BranchesModel.COL_PhoneNumber.Name, model.PhoneNumber),
                DBConnection.getSqlParameter(BranchesModel.COL_InvoiceHeaderText.Name, model.InvoiceHeaderText),
                DBConnection.getSqlParameter(BranchesModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(BranchesModel.COL_Notes.Name, model.Notes)
            );
            ActivityLogsController.AddCreateLog(db, Session, model.Id);
        }

        public void update(BranchesModel model, string log)
        {
            LIBWebMVC.WebDBConnection.Update(db.Database, "Branches",
                DBConnection.getSqlParameter(BranchesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(BranchesModel.COL_Name.Name, model.Name),
                DBConnection.getSqlParameter(BranchesModel.COL_Franchises_Id.Name, model.Franchises_Id),
                DBConnection.getSqlParameter(BranchesModel.COL_Address.Name, model.Address),
                DBConnection.getSqlParameter(BranchesModel.COL_PhoneNumber.Name, model.PhoneNumber),
                DBConnection.getSqlParameter(BranchesModel.COL_InvoiceHeaderText.Name, model.InvoiceHeaderText),
                DBConnection.getSqlParameter(BranchesModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(BranchesModel.COL_Notes.Name, model.Notes)
            );
            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
        }

        /******************************************************************************************************************************************************/
    }
}