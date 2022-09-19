﻿using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class ClubSchedulesController : Controller
    {
        private readonly DBContext db = new DBContext();
        private const int CLUBCLASSONLINELINKS_WEEKSTODISPLAY = 2;

        /* INDEX **********************************************************************************************************************************************/

        public ActionResult Index(int? rss, string FILTER_Keyword, DayOfWeekEnum? FILTER_DayOfWeek, DateTime? FILTER_StartTime, DateTime? FILTER_EndTime, Guid? FILTER_Languages_Id, int? FILTER_OnSite)
        {
            if (!UserAccountsController.getUserAccess(Session).ClubSchedules_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (FILTER_StartTime == null)
                FILTER_StartTime = Util.standardizeTimeToIgnoreDate("00_00");

            if (FILTER_EndTime == null)
                FILTER_EndTime = Util.standardizeTimeToIgnoreDate("23_59");

            setViewBag(FILTER_Keyword, FILTER_DayOfWeek, FILTER_StartTime, FILTER_EndTime, FILTER_Languages_Id, FILTER_OnSite);

            if (rss != null && !SettingsController.ShowOnlyOwnUserData(Session))
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                return View(get(FILTER_Keyword, FILTER_DayOfWeek, FILTER_StartTime, FILTER_EndTime, FILTER_Languages_Id, FILTER_OnSite));
            }
        }

        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, DayOfWeekEnum? FILTER_DayOfWeek, DateTime? FILTER_StartTime, DateTime? FILTER_EndTime, Guid? FILTER_Languages_Id, int? FILTER_OnSite)
        {
            setViewBag(FILTER_Keyword, FILTER_DayOfWeek, FILTER_StartTime, FILTER_EndTime, FILTER_Languages_Id, FILTER_OnSite);
            return View(get(FILTER_Keyword, FILTER_DayOfWeek, FILTER_StartTime, FILTER_EndTime, FILTER_Languages_Id, FILTER_OnSite));
        }

        /* CREATE *********************************************************************************************************************************************/

        public ActionResult Create(string FILTER_Keyword, DayOfWeekEnum? FILTER_DayOfWeek, DateTime? FILTER_StartTime, DateTime? FILTER_EndTime, DayOfWeekEnum? DayOfWeek, string StartTime, string EndTime, Guid? Id, Guid? FILTER_Languages_Id, int? FILTER_OnSite)
        {
            if (!UserAccountsController.getUserAccess(Session).ClubSchedules_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_DayOfWeek, FILTER_StartTime, FILTER_EndTime, FILTER_Languages_Id, FILTER_OnSite);
            ClubSchedulesModel model = new ClubSchedulesModel();
            if (DayOfWeek != null) model.DayOfWeek = (DayOfWeekEnum)DayOfWeek;
            if (!string.IsNullOrEmpty(StartTime))
            {
                model.StartTime = (DateTime)Util.standardizeTimeToIgnoreDate(StartTime);
            }
            if (!string.IsNullOrEmpty(StartTime))
            {
                model.EndTime = (DateTime)Util.standardizeTimeToIgnoreDate(EndTime);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClubSchedulesModel model, string FILTER_Keyword, DayOfWeekEnum? FILTER_DayOfWeek, DateTime? FILTER_StartTime, DateTime? FILTER_EndTime, Guid? FILTER_Languages_Id, int? FILTER_OnSite)
        {
            if (ModelState.IsValid)
            {
                standardizeTimeToIgnoreDate(model);
                model.Branches_Id = Helper.getActiveBranchId(Session);
                if (isExists(null, model.ClubClasses_Id, model.DayOfWeek, model.StartTime, model.EndTime))
                    ModelState.AddModelError(ClubSchedulesModel.COL_DayOfWeek.Name, "Kombinasi sudah terdaftar");
                else
                {
                    add(model);
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_DayOfWeek = FILTER_DayOfWeek, FILTER_StartTime = FILTER_StartTime, FILTER_EndTime = FILTER_EndTime, FILTER_Languages_Id = FILTER_Languages_Id, FILTER_OnSite = FILTER_OnSite });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_DayOfWeek, FILTER_StartTime, FILTER_EndTime, FILTER_Languages_Id, FILTER_OnSite);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        public ActionResult Edit(Guid? id, string FILTER_Keyword, DayOfWeekEnum? FILTER_DayOfWeek, DateTime? FILTER_StartTime, DateTime? FILTER_EndTime, Guid? FILTER_Languages_Id, int? FILTER_OnSite)
        {
            if (!UserAccountsController.getUserAccess(Session).ClubSchedules_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_DayOfWeek, FILTER_StartTime, FILTER_EndTime, FILTER_Languages_Id, FILTER_OnSite);
            ClubSchedulesModel model = get((Guid)id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClubSchedulesModel modifiedModel, string FILTER_Keyword, DayOfWeekEnum? FILTER_DayOfWeek, DateTime? FILTER_StartTime, DateTime? FILTER_EndTime, Guid? FILTER_Languages_Id, int? FILTER_OnSite)
        {
            if (ModelState.IsValid)
            {
                standardizeTimeToIgnoreDate(modifiedModel);
                if (isExists(modifiedModel.Id, modifiedModel.ClubClasses_Id, modifiedModel.DayOfWeek, modifiedModel.StartTime, modifiedModel.EndTime))
                    ModelState.AddModelError(ClubSchedulesModel.COL_DayOfWeek.Name, "Kombinasi sudah terdaftar");
                else
                {
                    ClubSchedulesModel originalModel = get(modifiedModel.Id);

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.DayOfWeek, modifiedModel.DayOfWeek, ClubSchedulesModel.COL_DayOfWeek.LogDisplay);
                    log = Helper.append(log, originalModel.StartTime, modifiedModel.StartTime, ClubSchedulesModel.COL_StartTime.LogDisplay);
                    log = Helper.append(log, originalModel.EndTime, modifiedModel.EndTime, ClubSchedulesModel.COL_EndTime.LogDisplay);
                    log = Helper.append<ClubClassesModel>(log, originalModel.ClubClasses_Id, modifiedModel.ClubClasses_Id, ClubSchedulesModel.COL_ClubClasses_Id.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, ClubSchedulesModel.COL_Active.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, ClubSchedulesModel.COL_Notes.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                        update(modifiedModel, log);

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword, FILTER_DayOfWeek = FILTER_DayOfWeek, FILTER_StartTime = FILTER_StartTime, FILTER_EndTime = FILTER_EndTime, FILTER_Languages_Id = FILTER_Languages_Id, FILTER_OnSite = FILTER_OnSite });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_DayOfWeek, FILTER_StartTime, FILTER_EndTime, FILTER_Languages_Id, FILTER_OnSite);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, DayOfWeekEnum? FILTER_DayOfWeek, DateTime? FILTER_StartTime, DateTime? FILTER_EndTime, Guid? FILTER_Languages_Id, int? FILTER_OnSite)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_DayOfWeek = FILTER_DayOfWeek;
            ViewBag.FILTER_StartTime = FILTER_StartTime;
            ViewBag.FILTER_EndTime = FILTER_EndTime;
            ViewBag.FILTER_Languages_Id = FILTER_Languages_Id;
            ViewBag.FILTER_OnSite = FILTER_OnSite;
            LanguagesController.setDropDownListViewBag(this);
            ClubClassesController.setDropDownListViewBag(this);
        }

        public static void standardizeTimeToIgnoreDate(ClubSchedulesModel model)
        {
            model.StartTime = Util.standardizeTimeToIgnoreDate(model.StartTime);
            model.EndTime = Util.standardizeTimeToIgnoreDate(model.EndTime);
        }

        public JsonResult Ajax_Delete(Guid id)
        {
            delete(id);
            return Json(new { Message = "" });
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, Guid ClubClasses_Id, DayOfWeekEnum DayOfWeek, DateTime StartTime, DateTime EndTime)
        {
            return db.Database.SqlQuery<ClubSchedulesModel>(@"
                        SELECT ClubSchedules.*
                        FROM ClubSchedules
                        WHERE 1=1 
							AND (@Id IS NULL OR ClubSchedules.Id <> @Id)
                            AND ClubSchedules.ClubClasses_Id = @ClubClasses_Id
                            AND ClubSchedules.[DayOfWeek] = @DayOfWeek
                            AND ((@StartTime >= ClubSchedules.StartTime AND @StartTime < ClubSchedules.EndTime)
                                OR (@EndTime > ClubSchedules.StartTime AND @EndTime <= ClubSchedules.EndTime)
                            )
                    ",
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_ClubClasses_Id.Name, ClubClasses_Id),
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_DayOfWeek.Name, DayOfWeek),
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_StartTime.Name, StartTime),
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_EndTime.Name, EndTime)
                ).Count() > 0;
        }

        public List<ClubSchedulesModel> get(string FILTER_Keyword, DayOfWeekEnum? FILTER_DayOfWeek, DateTime? FILTER_StartTime, DateTime? FILTER_EndTime, Guid? FILTER_Languages_Id, int? FILTER_OnSite) 
        {
            return get(Session, null, FILTER_DayOfWeek, FILTER_StartTime, FILTER_EndTime, FILTER_Keyword, FILTER_Languages_Id, FILTER_OnSite); 
        }
        public static List<ClubSchedulesModel> get(HttpSessionStateBase Session, DayOfWeekEnum? DayOfWeek, DateTime? StartTime, DateTime? EndTime)
        { return get(Session, null, DayOfWeek, StartTime, EndTime, null, null, null); }
        public ClubSchedulesModel get(Guid Id) { return get(Session, Id, null, null, null, null, null, null).FirstOrDefault(); }
        public List<ClubSchedulesModel> get() { return get(Session, null, null, null, null, null, null, null); }
        public static List<ClubSchedulesModel> get(HttpSessionStateBase Session, Guid? Id, DayOfWeekEnum? DayOfWeek, DateTime? StartTime, DateTime? EndTime,
            string FILTER_Keyword, Guid? FILTER_Languages_Id, int? FILTER_OnSite)
        {
            if (StartTime != null)
                StartTime = Util.standardizeTimeToIgnoreDate((DateTime)StartTime);
            if (EndTime != null)
                EndTime = Util.standardizeTimeToIgnoreDate((DateTime)EndTime);

            List<ClubSchedulesModel> models = new DBContext().Database.SqlQuery<ClubSchedulesModel>(@"
                        SELECT ClubSchedules.*,
                            Languages.Name AS Languages_Name,
                            Branches.Name AS Branches_Name,
                            ClubClasses.Name AS ClubClasses_Name
                        FROM ClubSchedules
                            LEFT JOIN ClubClasses ON ClubClasses.Id = ClubSchedules.ClubClasses_Id
                            LEFT JOIN Languages ON Languages.Id = ClubClasses.Languages_Id
                            LEFT JOIN Branches ON Branches.Id = ClubSchedules.Branches_Id
                        WHERE 1=1
							AND (@Id IS NULL OR ClubSchedules.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@DayOfWeek IS NULL OR ClubSchedules.[DayOfWeek] = @DayOfWeek)
                                AND ((@StartTime IS NULL OR (@StartTime >= ClubSchedules.StartTime OR @StartTime <= ClubSchedules.EndTime))
                                    OR (@EndTime IS NULL OR (@EndTime >= ClubSchedules.StartTime OR @EndTime <= ClubSchedules.EndTime))
                                    )
    							AND (@FILTER_Keyword IS NULL OR (
                                    ClubSchedules.DayOfWeek LIKE '%'+@FILTER_Keyword+'%'
                                    OR ClubClasses.Name LIKE '%'+@FILTER_Keyword+'%'
                                ))
                                AND (@FILTER_Languages_Id IS NULL OR ClubClasses.Languages_Id = @FILTER_Languages_Id)
                                AND (@FILTER_OnSite IS NULL OR ClubSchedules.OnSite = @FILTER_OnSite)
                                AND (@Branches_Id IS NULL OR ClubSchedules.Branches_Id = @Branches_Id)
                            ))
						ORDER BY ClubSchedules.DayOfWeek ASC, ClubSchedules.StartTime ASC, ClubSchedules.EndTime ASC, Languages.Name ASC
                    ",
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_DayOfWeek.Name, DayOfWeek),
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_StartTime.Name, StartTime),
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_EndTime.Name, EndTime),
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_Branches_Id.Name, null),// Helper.getActiveBranchId(Session)),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                    DBConnection.getSqlParameter("FILTER_Languages_Id", FILTER_Languages_Id),
                    DBConnection.getSqlParameter("FILTER_OnSite", FILTER_OnSite)
                ).ToList();

            DateTime DisplayStartDate = (DateTime)Util.getAsStartDate(DateTime.Now);
            DateTime DisplayEndDate = (DateTime)Util.getAsEndDate(DateTime.Now.AddDays(CLUBCLASSONLINELINKS_WEEKSTODISPLAY * 7));
            List<ClubClassOnlineLinksModel> ClubClassOnlineLinksModels = ClubClassOnlineLinksController.get(DisplayStartDate, DisplayEndDate);
            int Index;
            foreach (ClubClassOnlineLinksModel clubClassOnlineLinksModel in ClubClassOnlineLinksModels)
            {
                Index = models.FindIndex(x => x.ClubClasses_Id == clubClassOnlineLinksModel.ClubClasses_Id);
                if (Index > -1)
                    models[Index].ClubClassOnlineLinks.Add(clubClassOnlineLinksModel);
            }

            return models;
        }

        public void add(ClubSchedulesModel model)
        {
            LIBWebMVC.WebDBConnection.Insert(db.Database, "ClubSchedules",
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_ClubClasses_Id.Name, model.ClubClasses_Id),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_DayOfWeek.Name, model.DayOfWeek),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_StartTime.Name, model.StartTime),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_EndTime.Name, model.EndTime),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_OnSite.Name, model.OnSite),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_Branches_Id.Name, model.Branches_Id),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_Notes.Name, model.Notes)
            );
            ActivityLogsController.AddCreateLog(db, Session, model.Id);
            db.SaveChanges();
        }

        public void update(ClubSchedulesModel model, string log)
        {
            LIBWebMVC.WebDBConnection.Update(db.Database, "ClubSchedules",
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_ClubClasses_Id.Name, model.ClubClasses_Id),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_DayOfWeek.Name, model.DayOfWeek),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_StartTime.Name, model.StartTime),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_EndTime.Name, model.EndTime),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_OnSite.Name, model.OnSite),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_Notes.Name, model.Notes)
            );
            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
            db.SaveChanges();
        }

        public void delete(Guid Id)
        {
            LIBWebMVC.WebDBConnection.Delete(db.Database, "ClubSchedules",
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_Id.Name, Id)
            );
        }

        /******************************************************************************************************************************************************/
    }
}