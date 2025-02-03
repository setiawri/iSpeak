using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;
using LIBWebMVC;

namespace iSpeakWebApp.Controllers
{
    /*
     * HourlyRates is filtered by Franchise. 
     */

    public class HourlyRatesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            LessonPackagesController.setDropDownListViewBag(this);
            BranchesController.setDropDownListViewBag(this);
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: HourlyRates
        public ActionResult Index(int? rss, string FILTER_Keyword)
        {
            if (!UserAccountsController.getUserAccess(Session).HourlyRates_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                return View(get(FILTER_Keyword));
            }
        }

        // POST: HourlyRates
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword)
        {
            setViewBag(FILTER_Keyword);
            return View(get(FILTER_Keyword));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: HourlyRates/Create
        public ActionResult Create(string FILTER_Keyword)
        {
            if (!UserAccountsController.getUserAccess(Session).HourlyRates_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword);
            return View(new HourlyRatesModel());
        }

        // POST: HourlyRates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HourlyRatesModel model, string FILTER_Keyword)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model))
                    UtilWebMVC.setBootboxMessage(this, "Kombinasi sudah terdaftar");
                else
                {
                    add(model);
                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword });
                }
            }

            setViewBag(FILTER_Keyword);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: HourlyRates/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword)
        {
            if (!UserAccountsController.getUserAccess(Session).HourlyRates_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword);
            return View(get((Guid)id));
        }

        // POST: HourlyRates/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HourlyRatesModel modifiedModel, string FILTER_Keyword)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel))
                    UtilWebMVC.setBootboxMessage(this, "Kombinasi sudah terdaftar");
                else
                {
                    HourlyRatesModel originalModel = get(modifiedModel.Id);

                    string log = string.Empty;
                    log = Helper.append<BranchesModel>(log, originalModel.Branches_Id, modifiedModel.Branches_Id, HourlyRatesModel.COL_Branches_Id.LogDisplay);
                    log = Helper.append<LessonPackagesModel>(log, originalModel.LessonPackages_Id, modifiedModel.LessonPackages_Id, HourlyRatesModel.COL_LessonPackages_Id.LogDisplay);
                    log = Helper.append<UserAccountsModel>(log, originalModel.UserAccounts_Id, modifiedModel.UserAccounts_Id, HourlyRatesModel.COL_UserAccounts_Id.LogDisplay);
                    log = Helper.append(log, originalModel.Rate, modifiedModel.Rate, HourlyRatesModel.COL_Rate.LogDisplay);
                    log = Helper.append(log, originalModel.FullTimeTutorPayrate, modifiedModel.FullTimeTutorPayrate, HourlyRatesModel.COL_FullTimeTutorPayrate.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, HourlyRatesModel.COL_Notes.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                        update(modifiedModel, log);

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword });
                }
            }

            setViewBag(FILTER_Keyword);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, HourlyRatesModel model)
        {
            return db.Database.SqlQuery<HourlyRatesModel>(@"
                        SELECT HourlyRates.*
                        FROM HourlyRates
                        WHERE 1=1                             
							AND (@Id IS NULL OR HourlyRates.Id <> @Id)
							AND (@Branches_Id IS NULL OR HourlyRates.Branches_Id = Branches_Id)
                            AND (@UserAccounts_Id IS NULL OR HourlyRates.UserAccounts_Id = @UserAccounts_Id)
                            AND (@LessonPackages_Id IS NULL OR HourlyRates.LessonPackages_Id = @LessonPackages_Id)
                            AND (@Rate = 0 OR HourlyRates.Rate > 0)
                            AND (@Rate > 0 OR HourlyRates.Rate = 0)
                            AND (@FullTimeTutorPayrate = 0 OR HourlyRates.FullTimeTutorPayrate > 0)
                            AND (@FullTimeTutorPayrate > 0 OR HourlyRates.FullTimeTutorPayrate = 0)
                    ",
                    DBConnection.getSqlParameter(HourlyRatesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(HourlyRatesModel.COL_Branches_Id.Name, model.Branches_Id),
                    DBConnection.getSqlParameter(HourlyRatesModel.COL_UserAccounts_Id.Name, model.UserAccounts_Id),
                    DBConnection.getSqlParameter(HourlyRatesModel.COL_LessonPackages_Id.Name, model.LessonPackages_Id),
                    DBConnection.getSqlParameter(HourlyRatesModel.COL_Rate.Name, model.Rate),
                    DBConnection.getSqlParameter(HourlyRatesModel.COL_FullTimeTutorPayrate.Name, model.FullTimeTutorPayrate)
                ).Count() > 0;
        }

        public List<HourlyRatesModel> get(string FILTER_Keyword) { return get(null, FILTER_Keyword, null); }
        public HourlyRatesModel get(Guid Id) { return get(Id, null, null).FirstOrDefault(); }
        public static List<HourlyRatesModel> get(Guid? Id, string FILTER_Keyword, Guid? UserAccounts_Id)
        {
            return new DBContext().Database.SqlQuery<HourlyRatesModel>(@"
                        SELECT HourlyRates.*,
                            UserAccounts.Fullname AS UserAccounts_Fullname,
                            LessonPackages.Name AS LessonPackages_Name,
                            Branches.Name AS Branches_Name
                        FROM HourlyRates
                            LEFT JOIN UserAccounts ON UserAccounts.Id = HourlyRates.UserAccounts_Id
                            LEFT JOIN LessonPackages ON LessonPackages.Id = HourlyRates.LessonPackages_Id
                            LEFT JOIN Branches ON Branches.Id = HourlyRates.Branches_Id
                        WHERE 1=1
							AND (@Id IS NULL OR HourlyRates.Id = @Id)
							AND (@Id IS NOT NULL OR (
    							(@FILTER_Keyword IS NULL OR (
                                    UserAccounts.Fullname LIKE '%'+@FILTER_Keyword+'%'
                                    OR LessonPackages.Name LIKE '%'+@FILTER_Keyword+'%'
                                ))
    							AND (@UserAccounts_Id IS NULL OR UserAccounts.Id = @UserAccounts_Id)
                            ))
						ORDER BY UserAccounts.Fullname ASC
                    ",
                    DBConnection.getSqlParameter(HourlyRatesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                    DBConnection.getSqlParameter(HourlyRatesModel.COL_UserAccounts_Id.Name, UserAccounts_Id)
                ).ToList();
        }

        public void update(HourlyRatesModel model, string log)
        {
            WebDBConnection.Update(db.Database, "HourlyRates",
                DBConnection.getSqlParameter(HourlyRatesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(HourlyRatesModel.COL_Branches_Id.Name, model.Branches_Id),
                DBConnection.getSqlParameter(HourlyRatesModel.COL_LessonPackages_Id.Name, model.LessonPackages_Id),
                DBConnection.getSqlParameter(HourlyRatesModel.COL_UserAccounts_Id.Name, model.UserAccounts_Id),
                DBConnection.getSqlParameter(HourlyRatesModel.COL_Rate.Name, model.Rate),
                DBConnection.getSqlParameter(HourlyRatesModel.COL_FullTimeTutorPayrate.Name, model.FullTimeTutorPayrate),
                DBConnection.getSqlParameter(HourlyRatesModel.COL_Notes.Name, model.Notes)
            );

            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
        }

        public void add(HourlyRatesModel model)
        {
            model.Id = Guid.NewGuid();

            WebDBConnection.Insert(db.Database, "HourlyRates",
                DBConnection.getSqlParameter(HourlyRatesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(HourlyRatesModel.COL_Branches_Id.Name, model.Branches_Id),
                DBConnection.getSqlParameter(HourlyRatesModel.COL_LessonPackages_Id.Name, model.LessonPackages_Id),
                DBConnection.getSqlParameter(HourlyRatesModel.COL_UserAccounts_Id.Name, model.UserAccounts_Id),
                DBConnection.getSqlParameter(HourlyRatesModel.COL_Rate.Name, model.Rate),
                DBConnection.getSqlParameter(HourlyRatesModel.COL_FullTimeTutorPayrate.Name, model.FullTimeTutorPayrate),
                DBConnection.getSqlParameter(HourlyRatesModel.COL_Notes.Name, model.Notes)
            );

            ActivityLogsController.AddCreateLog(db, Session, model.Id);
        }

        public static List<HourlyRatesModel> getActiveFullTimeEmployeePayrates(HttpSessionStateBase Session)
        {
            return new DBContext().Database.SqlQuery<HourlyRatesModel>(@"
                        SELECT HourlyRates.*,
							UserAccounts.Fullname AS UserAccounts_FullName
						FROM HourlyRates
							LEFT JOIN UserAccounts ON UserAccounts.Id = HourlyRates.UserAccounts_Id
						WHERE HourlyRates.FullTimeTutorPayrate > 0
							AND UserAccounts.Active = 1
							AND UserAccounts.Branches_Id = @Branches_Id
						ORDER BY UserAccounts.Fullname ASC
                    ",
                    DBConnection.getSqlParameter("Branches_Id", Helper.getActiveBranchId(Session))
                ).ToList();
        }

        /******************************************************************************************************************************************************/
    }
}