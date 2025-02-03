using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;
using LIBWebMVC;

namespace iSpeakWebApp.Controllers
{
    /*
     * Vouchers is filtered by Franchise. 
     */

    public class VouchersController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: Vouchers
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Vouchers_View)
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

        // POST: Vouchers
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active)
        {
            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get(FILTER_Keyword, FILTER_Active));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: Vouchers/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Vouchers_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(new VouchersModel());
        }

        // POST: Vouchers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VouchersModel model, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Code))
                    ModelState.AddModelError(VouchersModel.COL_Code.Name, $"{model.Code} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    model.Active = true;
                    add(model);
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: Vouchers/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).Vouchers_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get((Guid)id));
        }

        // POST: Vouchers/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VouchersModel modifiedModel, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Code))
                    ModelState.AddModelError(VouchersModel.COL_Code.Name, $"{modifiedModel.Code} sudah terdaftar");
                else
                {
                    VouchersModel originalModel = get(modifiedModel.Id);

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.Code, modifiedModel.Code, VouchersModel.COL_Code.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, VouchersModel.COL_Active.LogDisplay);
                    log = Helper.append(log, originalModel.Description, modifiedModel.Description, VouchersModel.COL_Description.LogDisplay);
                    log = Helper.append(log, originalModel.Amount, modifiedModel.Amount, VouchersModel.COL_Amount.LogDisplay);

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
            controller.ViewBag.Vouchers = new SelectList(get(1), VouchersModel.COL_Id.Name, VouchersModel.COL_DDLDescription.Name);
        }

        public static void setViewBag(Controller controller)
        {
            controller.ViewBag.VouchersModels = get(1);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Code)
        {
            return db.Database.SqlQuery<VouchersModel>(@"
                        SELECT Vouchers.*,
                            '' AS DDLDescription
                        FROM Vouchers
                        WHERE 1=1 
							AND (@Id IS NOT NULL OR Vouchers.Code = @Code)
							AND (@Id IS NULL OR (Vouchers.Code = @Code AND Vouchers.Id <> @Id))
                    ",
                    DBConnection.getSqlParameter(VouchersModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(VouchersModel.COL_Code.Name, Code)
                ).Count() > 0;
        }

        public List<VouchersModel> get(string FILTER_Keyword, int? FILTER_Active) { return get(null, FILTER_Keyword, FILTER_Active); }
        public VouchersModel get(Guid Id) { return get(Id, null, null).FirstOrDefault(); }
        public static List<VouchersModel> get(int? Active) { return get(null, null, Active); }
        public static List<VouchersModel> get(Guid? Id, string FILTER_Keyword, int? Active)
        {
            return new DBContext().Database.SqlQuery<VouchersModel>(@"
                        SELECT Vouchers.*,
                            Vouchers.Code + ' (' + FORMAT(Vouchers.Amount,'N0') + ')' AS DDLDescription
                        FROM Vouchers
                        WHERE 1=1
							AND (@Id IS NULL OR Vouchers.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Active IS NULL OR Vouchers.Active = @Active)
    							AND (@FILTER_Keyword IS NULL OR (Vouchers.Code LIKE '%'+@FILTER_Keyword+'%' OR Vouchers.Description LIKE '%'+@FILTER_Keyword+'%'))
                            ))
						ORDER BY Vouchers.Code ASC
                    ",
                    DBConnection.getSqlParameter(VouchersModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(VouchersModel.COL_Active.Name, Active),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
                ).ToList();
        }

        public void update(VouchersModel model, string log)
        {
            WebDBConnection.Update(db.Database, "Vouchers",
                    DBConnection.getSqlParameter(VouchersModel.COL_Id.Name, model.Id),
                    DBConnection.getSqlParameter(VouchersModel.COL_Active.Name, model.Active),
                    DBConnection.getSqlParameter(VouchersModel.COL_Code.Name, model.Code),
                    DBConnection.getSqlParameter(VouchersModel.COL_Description.Name, model.Description),
                    DBConnection.getSqlParameter(VouchersModel.COL_Amount.Name, model.Amount)
                );
            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
        }

        public void add(VouchersModel model)
        {
            WebDBConnection.Insert(db.Database, "Vouchers",
                    DBConnection.getSqlParameter(VouchersModel.COL_Id.Name, model.Id),
                    DBConnection.getSqlParameter(VouchersModel.COL_Active.Name, model.Active),
                    DBConnection.getSqlParameter(VouchersModel.COL_Code.Name, model.Code),
                    DBConnection.getSqlParameter(VouchersModel.COL_Description.Name, model.Description),
                    DBConnection.getSqlParameter(VouchersModel.COL_Amount.Name, model.Amount)
                );
            ActivityLogsController.AddCreateLog(db, Session, model.Id);
        }

        /******************************************************************************************************************************************************/
    }
}