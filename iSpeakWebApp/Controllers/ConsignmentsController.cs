using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class ConsignmentsController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: Consignments
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Branches_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).Consignments_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Branches_Id);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                return View(get(FILTER_Keyword, FILTER_Active, FILTER_Branches_Id));
            }
        }

        // POST: Consignments
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Branches_Id)
        {
            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Branches_Id);
            return View(get(FILTER_Keyword, FILTER_Active, FILTER_Branches_Id));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: Consignments/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Branches_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).Consignments_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Branches_Id);
            return View(new ConsignmentsModel());
        }

        // POST: Consignments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ConsignmentsModel model, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Branches_Id)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Name))
                    ModelState.AddModelError(ConsignmentsModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    add(model);
                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active, FILTER_Branches_Id = FILTER_Branches_Id });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Branches_Id);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: Consignments/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Branches_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).Consignments_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Branches_Id);
            return View(get((Guid)id));
        }

        // POST: Consignments/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ConsignmentsModel modifiedModel, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Branches_Id)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(ConsignmentsModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    ConsignmentsModel originalModel = get(modifiedModel.Id);

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, ConsignmentsModel.COL_Name.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, ConsignmentsModel.COL_Active.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, ConsignmentsModel.COL_Notes.LogDisplay);
                    log = Helper.append<BranchesModel>(log, originalModel.Branches_Id, modifiedModel.Branches_Id, ConsignmentsModel.COL_Branches_Id.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                        update(modifiedModel, log);

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active, FILTER_Branches_Id = FILTER_Branches_Id });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_Branches_Id);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Branches_Id)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
            ViewBag.FILTER_Branches_Id = FILTER_Branches_Id;
            BranchesController.setDropDownListViewBag(this);
        }

        public static void setDropDownListViewBag(Controller controller)
        {
            controller.ViewBag.Consignments = new SelectList(get(), ConsignmentsModel.COL_Id.Name, ConsignmentsModel.COL_Name.Name);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Name)
        {
            return db.Database.SqlQuery<ConsignmentsModel>(@"
                        SELECT Consignments.*,
                            NULL AS Branches_Name
                        FROM Consignments
                        WHERE 1=1 
							AND (@Id IS NOT NULL OR Consignments.Name = @Name)
							AND (@Id IS NULL OR (Consignments.Name = @Name AND Consignments.Id <> @Id))
                    ",
                    DBConnection.getSqlParameter(ConsignmentsModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(ConsignmentsModel.COL_Name.Name, Name)
                ).Count() > 0;
        }

        public List<ConsignmentsModel> get(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_Branches_Id) { return get(null, FILTER_Active, FILTER_Keyword, FILTER_Branches_Id); }
        public ConsignmentsModel get(Guid Id) { return get(Id, null, null, null).FirstOrDefault(); }
        public static List<ConsignmentsModel> get() { return get(null, null, null, null); }
        public static List<ConsignmentsModel> get(Guid? Id, int? Active, string FILTER_Keyword, Guid? Branches_Id)
        {
            return new DBContext().Database.SqlQuery<ConsignmentsModel>(@"
                    SELECT Consignments.*,
                        Branches.Name AS Branches_Name
                    FROM Consignments
                        LEFT JOIN Branches ON Branches.Id = Consignments.Branches_Id
                    WHERE 1=1
						AND (@Id IS NULL OR Consignments.Id = @Id)
						AND (@Id IS NOT NULL OR (
                            (@Active IS NULL OR Consignments.Active = @Active)
    						AND (@FILTER_Keyword IS NULL OR (Consignments.Name LIKE '%'+@FILTER_Keyword+'%'))
                            AND (@Branches_Id IS NULL OR Consignments.Branches_Id = @Branches_Id)
                        ))
					ORDER BY Consignments.Name ASC
                ",
                DBConnection.getSqlParameter(ConsignmentsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(ConsignmentsModel.COL_Active.Name, Active),
                DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                DBConnection.getSqlParameter(ConsignmentsModel.COL_Branches_Id.Name, Branches_Id)
            ).ToList();
        }

        public void update(ConsignmentsModel model, string log)
        {
            LIBWebMVC.WebDBConnection.Update(db.Database, "Consignments",
                DBConnection.getSqlParameter(ConsignmentsModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(ConsignmentsModel.COL_Name.Name, model.Name),
                DBConnection.getSqlParameter(ConsignmentsModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(ConsignmentsModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(ConsignmentsModel.COL_Branches_Id.Name, model.Branches_Id)
            );

            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
            db.SaveChanges();
        }

        public void add(ConsignmentsModel model)
        {
            LIBWebMVC.WebDBConnection.Insert(db.Database, "Consignments",
                DBConnection.getSqlParameter(ConsignmentsModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(ConsignmentsModel.COL_Name.Name, model.Name),
                DBConnection.getSqlParameter(ConsignmentsModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(ConsignmentsModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(ConsignmentsModel.COL_Branches_Id.Name, model.Branches_Id)
            );

            ActivityLogsController.AddCreateLog(db, Session, model.Id);
            db.SaveChanges();
        }

        /******************************************************************************************************************************************************/
    }
}