using System;
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

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
        }

        /* INDEX **********************************************************************************************************************************************/

        public ActionResult Index(int? rss, string FILTER_Keyword, string FILTER_Custom)
        {
            if (!UserAccountsController.getUserAccess(Session).ClubSchedules_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                return View(get(FILTER_Keyword, FILTER_Custom));
            }
        }

        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, string FILTER_Custom)
        {
            setViewBag(FILTER_Keyword);
            return View(get(FILTER_Keyword, FILTER_Custom));
        }

        /* CREATE *********************************************************************************************************************************************/

        public ActionResult Create(string FILTER_Keyword, string FILTER_Custom, DayOfWeekEnum? DayOfWeek, string StartTime, Guid? Id)
        {
            if (!UserAccountsController.getUserAccess(Session).ClubSchedules_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword);
            ClubSchedulesModel model = new ClubSchedulesModel();
            model.Branches_Id = Helper.getActiveBranchId(Session);
            if (DayOfWeek != null) model.DayOfWeek = (DayOfWeekEnum)DayOfWeek;
            if (!string.IsNullOrEmpty(StartTime))
            {
                model.StartTime = Util.standardizeTimeToIgnoreDate(StartTime);
                model.EndTime = model.StartTime.AddHours(1);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClubSchedulesModel model, string FILTER_Keyword, string FILTER_Custom)
        {
            if (ModelState.IsValid)
            {
                standardizeTimeToIgnoreDate(model);
                if (isExists(null, model.Description, model.DayOfWeek, model.StartTime, model.EndTime))
                    ModelState.AddModelError(ClubSchedulesModel.COL_DayOfWeek.Name, "Ada bentrok waktu dengan jadwal murid atau tutor");
                else
                {
                    add(model);
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_Custom = FILTER_Custom });
                }
            }

            setViewBag(FILTER_Keyword);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        public ActionResult Edit(Guid? id, string FILTER_Keyword, string FILTER_Custom)
        {
            if (!UserAccountsController.getUserAccess(Session).ClubSchedules_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword);
            ClubSchedulesModel model = get((Guid)id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClubSchedulesModel modifiedModel, string FILTER_Keyword, string FILTER_Custom)
        {
            if (ModelState.IsValid)
            {
                standardizeTimeToIgnoreDate(modifiedModel);
                if (isExists(modifiedModel.Id, modifiedModel.Description, modifiedModel.DayOfWeek, modifiedModel.StartTime, modifiedModel.EndTime))
                    ModelState.AddModelError(ClubSchedulesModel.COL_DayOfWeek.Name, "Kombinasi sudah terdaftar");
                else
                {
                    ClubSchedulesModel originalModel = get(modifiedModel.Id);

                    string log = string.Empty;
                    log = Helper.append<LessonPackagesModel>(log, originalModel.LessonPackages_Id, modifiedModel.LessonPackages_Id, ClubSchedulesModel.COL_LessonPackages_Id.LogDisplay);
                    log = Helper.append(log, originalModel.DayOfWeek, modifiedModel.DayOfWeek, ClubSchedulesModel.COL_DayOfWeek.LogDisplay);
                    log = Helper.append(log, originalModel.StartTime, modifiedModel.StartTime, ClubSchedulesModel.COL_StartTime.LogDisplay);
                    log = Helper.append(log, originalModel.EndTime, modifiedModel.EndTime, ClubSchedulesModel.COL_EndTime.LogDisplay);
                    log = Helper.append(log, originalModel.Description, modifiedModel.Description, ClubSchedulesModel.COL_Description.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, ClubSchedulesModel.COL_Active.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, ClubSchedulesModel.COL_Notes.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                        update(modifiedModel, log);

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword });
                }
            }

            setViewBag(FILTER_Keyword);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

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

        public static string generateFILTER_Custom(DayOfWeekEnum DayOfWeek, DateTime StartTime, DateTime EndTime)
        {
            return string.Format("{0}_{1:HH_mm}_{2:HH_mm}", (int)DayOfWeek, StartTime, EndTime);
        }

        public static void parseFILTER_Custom(string FILTER_Custom, ref DayOfWeekEnum? DayOfWeek, ref DateTime? StartTime, ref DateTime? EndTime)
        {
            List<int> filter = FILTER_Custom.Split('_').Select(Int32.Parse).ToList();
            DayOfWeek = (DayOfWeekEnum)filter[0];
            StartTime = new DateTime(1970, 1, 1, filter[1], filter[2], 0);
            EndTime = new DateTime(1970, 1, 1, filter[3], filter[4], 0);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Description, DayOfWeekEnum DayOfWeek, DateTime StartTime, DateTime EndTime)
        {
            return db.Database.SqlQuery<ClubSchedulesModel>(@"
                        SELECT ClubSchedules.*
                        FROM ClubSchedules
                        WHERE 1=1 
							AND (@Id IS NULL OR ClubSchedules.Id <> @Id)
                            AND ClubSchedules.Description = @Description
                            AND ClubSchedules.[DayOfWeek] = @DayOfWeek
                            AND ((@StartTime >= ClubSchedules.StartTime AND @StartTime < ClubSchedules.EndTime)
                                OR (@EndTime > ClubSchedules.StartTime AND @EndTime <= ClubSchedules.EndTime)
                            )
                    ",
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_Description.Name, Description),
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_DayOfWeek.Name, DayOfWeek),
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_StartTime.Name, StartTime),
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_EndTime.Name, EndTime)
                ).Count() > 0;
        }

        public List<ClubSchedulesModel> get(string FILTER_Keyword, string FILTER_Custom) 
        {
            DayOfWeekEnum? DayOfWeek = null;
            DateTime? StartTime = null;
            DateTime? EndTime = null;
            if (!string.IsNullOrEmpty(FILTER_Custom))
                parseFILTER_Custom(FILTER_Custom, ref DayOfWeek, ref StartTime, ref EndTime);

            return get(Session, null, null, DayOfWeek, StartTime, EndTime, FILTER_Keyword); 
        }
        public static List<ClubSchedulesModel> get(HttpSessionStateBase Session, Guid? LessonPackages_Id, DayOfWeekEnum? DayOfWeek, DateTime? StartTime, DateTime? EndTime)
        { return get(Session, null, LessonPackages_Id, DayOfWeek, StartTime, EndTime, null); }
        public ClubSchedulesModel get(Guid Id) { return get(Session, Id, null, null, null, null, null).FirstOrDefault(); }
        public List<ClubSchedulesModel> get() { return get(Session, null, null, null, null, null, null); }
        public static List<ClubSchedulesModel> get(HttpSessionStateBase Session, Guid? Id, Guid? LessonPackages_Id, DayOfWeekEnum? DayOfWeek, DateTime? StartTime, DateTime? EndTime,
            string FILTER_Keyword)
        {
            return new DBContext().Database.SqlQuery<ClubSchedulesModel>(@"
                        SELECT ClubSchedules.*,
                            Languages.Name AS Languages_Name
                        FROM ClubSchedules
                            LEFT JOIN LessonPackages ON LessonPackages.Id = ClubSchedules.LessonPackages_Id
                            LEFT JOIN Languages ON Languages.Id = LessonPackages.Languages_Id
                        WHERE 1=1
							AND (@Id IS NULL OR ClubSchedules.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@DayOfWeek IS NULL OR ClubSchedules.[DayOfWeek] = @DayOfWeek)
                                AND ((@StartTime IS NULL OR (@StartTime >= ClubSchedules.StartTime OR @StartTime <= ClubSchedules.EndTime))
                                    OR (@EndTime IS NULL OR (@EndTime >= ClubSchedules.StartTime OR @EndTime <= ClubSchedules.EndTime))
                                    )
    							AND (@FILTER_Keyword IS NULL OR (
                                    ClubSchedules.DayOfWeek LIKE '%'+@FILTER_Keyword+'%'
                                    OR ClubSchedules.Description LIKE '%'+@FILTER_Keyword+'%'
                                ))
                                AND (@Branches_Id IS NULL OR ClubSchedules.Branches_Id = @Branches_Id)
                            ))
						ORDER BY ClubSchedules.DayOfWeek ASC, Languages.Name ASC, ClubSchedules.StartTime ASC, ClubSchedules.EndTime ASC
                    ",
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_LessonPackages_Id.Name, LessonPackages_Id),
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_DayOfWeek.Name, DayOfWeek),
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_StartTime.Name, StartTime),
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_EndTime.Name, EndTime),
                    DBConnection.getSqlParameter("Branches_Id", Helper.getActiveBranchId(Session)),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
                ).ToList();
        }

        public void add(ClubSchedulesModel model)
        {
            LIBWebMVC.WebDBConnection.Insert(db.Database, "ClubSchedules",
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_Id.Name, model.Id),
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_LessonPackages_Id.Name, model.LessonPackages_Id),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_DayOfWeek.Name, model.DayOfWeek),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_StartTime.Name, model.StartTime),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_EndTime.Name, model.EndTime),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_Description.Name, model.Description),
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
                    DBConnection.getSqlParameter(ClubSchedulesModel.COL_LessonPackages_Id.Name, model.LessonPackages_Id),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_DayOfWeek.Name, model.DayOfWeek),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_StartTime.Name, model.StartTime),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_EndTime.Name, model.EndTime),
                DBConnection.getSqlParameter(ClubSchedulesModel.COL_Description.Name, model.Description),
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