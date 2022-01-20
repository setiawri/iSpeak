using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class TutorSchedulesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: TutorSchedules
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).TutorSchedules_View)
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

        // POST: TutorSchedules
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active)
        {
            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get(FILTER_Keyword, FILTER_Active));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: TutorSchedules/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).TutorSchedules_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(new TutorSchedulesModel());
        }

        // POST: TutorSchedules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TutorSchedulesModel model, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                standardizeTime(model);
                if (isExists(null, model.Tutor_UserAccounts_Id, model.DayOfWeek, model.StartTime, model.EndTime))
                    ModelState.AddModelError(TutorSchedulesModel.COL_Tutor_UserAccounts_Id.Name, "Kombinasi sudah terdaftar atau ada jam bentrok");
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

        // GET: TutorSchedules/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).TutorSchedules_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get((Guid)id));
        }

        // POST: TutorSchedules/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TutorSchedulesModel modifiedModel, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                standardizeTime(modifiedModel);
                if (isExists(modifiedModel.Id, modifiedModel.Tutor_UserAccounts_Id, modifiedModel.DayOfWeek, modifiedModel.StartTime, modifiedModel.EndTime))
                    ModelState.AddModelError(TutorSchedulesModel.COL_DayOfWeek.Name, "Kombinasi sudah terdaftar atau ada jam bentrok");
                else
                {
                    TutorSchedulesModel originalModel = get(modifiedModel.Id);

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.DayOfWeek, modifiedModel.DayOfWeek, TutorSchedulesModel.COL_DayOfWeek.LogDisplay);
                    log = Helper.append(log, originalModel.StartTime, modifiedModel.StartTime, TutorSchedulesModel.COL_StartTime.LogDisplay);
                    log = Helper.append(log, originalModel.EndTime, modifiedModel.EndTime, TutorSchedulesModel.COL_EndTime.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, TutorSchedulesModel.COL_Active.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, TutorSchedulesModel.COL_Notes.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                        update(modifiedModel, log);

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public static void standardizeTime(TutorSchedulesModel model)
        {
            model.StartTime = new DateTime(1970, 1, 1, model.StartTime.Hour, model.StartTime.Minute, 0);
            model.EndTime = new DateTime(1970, 1, 1, model.EndTime.Hour, model.EndTime.Minute, 0);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, Guid Tutor_UserAccounts_Id, DayOfWeekEnum DayOfWeek, DateTime StartTime, DateTime EndTime)
        {
            return db.Database.SqlQuery<TutorSchedulesModel>(@"
                        SELECT TutorSchedules.*
                        FROM TutorSchedules
                        WHERE 1=1 
							AND (@Id IS NULL OR TutorSchedules.Tutor_UserAccounts_Id = @Tutor_UserAccounts_Id)
							AND (@Id IS NOT NULL OR (
                                TutorSchedules.Tutor_UserAccounts_Id = @Tutor_UserAccounts_Id 
                                AND TutorSchedules.[DayOfWeek] = @DayOfWeek
                                AND (@StartTime IS NULL OR (TutorSchedules.StartTime <= @StartTime AND TutorSchedules.EndTime >= @StartTime))
                                AND (@EndTime IS NULL OR (TutorSchedules.StartTime <= @EndTime AND TutorSchedules.EndTime >= @EndTime))
                            ))
                    ",
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_Tutor_UserAccounts_Id.Name, Tutor_UserAccounts_Id),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_DayOfWeek.Name, DayOfWeek),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_StartTime.Name, StartTime),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_EndTime.Name, EndTime)
                ).Count() > 0;
        }

        public List<TutorSchedulesModel> get(string FILTER_Keyword, int? FILTER_Active) { return get(null, FILTER_Active, FILTER_Keyword); }
        public TutorSchedulesModel get(Guid Id) { return get(Id, null, null).FirstOrDefault(); }
        public static List<TutorSchedulesModel> get() { return get(null, null, null); }
        public static List<TutorSchedulesModel> get(Guid? Id, int? Active, string FILTER_Keyword)
        {
            return new DBContext().Database.SqlQuery<TutorSchedulesModel>(@"
                        SELECT TutorSchedules.*,
                            UserAccounts.Fullname AS Tutor_UserAccounts_Name
                        FROM TutorSchedules
                            LEFT JOIN UserAccounts ON UserAccounts.Id = TutorSchedules.Tutor_UserAccounts_Id
                        WHERE 1=1
							AND (@Id IS NULL OR TutorSchedules.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Active IS NULL OR TutorSchedules.Active = @Active)
    							AND (@FILTER_Keyword IS NULL OR (
                                    UserAccounts.Fullname LIKE '%'+@FILTER_Keyword+'%'
                                    OR TutorSchedules.DayOfWeek LIKE '%'+@FILTER_Keyword+'%'
                                ))
                            ))
						ORDER BY UserAccounts.Fullname ASC, TutorSchedules.DayOfWeek ASC, TutorSchedules.StartTime ASC, TutorSchedules.EndTime ASC
                    ",
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_Active.Name, Active),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
                ).ToList();
        }

        public void add(TutorSchedulesModel model)
        {
            LIBWebMVC.WebDBConnection.Insert(db.Database, "TutorSchedules",
                DBConnection.getSqlParameter(TutorSchedulesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(TutorSchedulesModel.COL_Tutor_UserAccounts_Id.Name, model.Tutor_UserAccounts_Id),
                DBConnection.getSqlParameter(TutorSchedulesModel.COL_DayOfWeek.Name, model.DayOfWeek),
                DBConnection.getSqlParameter(TutorSchedulesModel.COL_StartTime.Name, model.StartTime),
                DBConnection.getSqlParameter(TutorSchedulesModel.COL_EndTime.Name, model.EndTime),
                DBConnection.getSqlParameter(TutorSchedulesModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(TutorSchedulesModel.COL_Notes.Name, model.Notes)
            );
            ActivityLogsController.AddCreateLog(db, Session, model.Id);
            db.SaveChanges();
        }

        public void update(TutorSchedulesModel model, string log)
        {
            LIBWebMVC.WebDBConnection.Update(db.Database, "TutorSchedules",
                DBConnection.getSqlParameter(TutorSchedulesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(TutorSchedulesModel.COL_Tutor_UserAccounts_Id.Name, model.Tutor_UserAccounts_Id),
                DBConnection.getSqlParameter(TutorSchedulesModel.COL_DayOfWeek.Name, model.DayOfWeek),
                DBConnection.getSqlParameter(TutorSchedulesModel.COL_StartTime.Name, model.StartTime),
                DBConnection.getSqlParameter(TutorSchedulesModel.COL_EndTime.Name, model.EndTime),
                DBConnection.getSqlParameter(TutorSchedulesModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(TutorSchedulesModel.COL_Notes.Name, model.Notes)
            );
            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
            db.SaveChanges();
        }

        /******************************************************************************************************************************************************/
    }
}