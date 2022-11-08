using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class ClubClassOnlineLinksController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_ClubClasses_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).ClubClassOnlineLinks_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_ClubClasses_Id);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                return View(get(FILTER_Keyword, FILTER_Active, FILTER_ClubClasses_Id));
            }
        }

        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_ClubClasses_Id)
        {
            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_ClubClasses_Id);
            return View(get(FILTER_Keyword, FILTER_Active, FILTER_ClubClasses_Id));
        }

        /* CREATE *********************************************************************************************************************************************/

        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_ClubClasses_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).ClubClassOnlineLinks_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_ClubClasses_Id);
            return View(new ClubClassOnlineLinksModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClubClassOnlineLinksModel model, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_ClubClasses_Id)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Name, model.ClubClasses_Id))
                    ModelState.AddModelError(ClubClassOnlineLinksModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    add(model);
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active, FILTER_ClubClasses_Id = FILTER_ClubClasses_Id });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_ClubClasses_Id);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_ClubClasses_Id)
        {
            if (!UserAccountsController.getUserAccess(Session).ClubClassOnlineLinks_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_ClubClasses_Id);
            return View(get((Guid)id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClubClassOnlineLinksModel modifiedModel, string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_ClubClasses_Id)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Name, modifiedModel.ClubClasses_Id))
                    ModelState.AddModelError(ClubClassOnlineLinksModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else
                {
                    ClubClassOnlineLinksModel originalModel = get(modifiedModel.Id);

                    string log = string.Empty;
                    log = Helper.append<ClubClassesModel>(log, originalModel.ClubClasses_Id, modifiedModel.ClubClasses_Id, ClubClassOnlineLinksModel.COL_ClubClasses_Id.LogDisplay);
                    log = Helper.append(log, originalModel.Name, modifiedModel.Name, ClubClassOnlineLinksModel.COL_Name.LogDisplay);
                    log = Helper.append(log, originalModel.OnlineLink, modifiedModel.OnlineLink, ClubClassOnlineLinksModel.COL_OnlineLink.LogDisplay);
                    log = Helper.append(log, originalModel.WeekNo, modifiedModel.WeekNo, ClubClassOnlineLinksModel.COL_WeekNo.LogDisplay);
                    log = Helper.append(log, originalModel.DurationDays, modifiedModel.DurationDays, ClubClassOnlineLinksModel.COL_DurationDays.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, ClubClassOnlineLinksModel.COL_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, ClubClassOnlineLinksModel.COL_Active.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                        update(modifiedModel, log);

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active, FILTER_ClubClasses_Id = FILTER_ClubClasses_Id });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active, FILTER_ClubClasses_Id);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_ClubClasses_Id)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
            ViewBag.FILTER_ClubClasses_Id = FILTER_ClubClasses_Id;
            ClubClassesController.setDropDownListViewBag(this);
        }

        public static void setDropDownListViewBag(Controller controller)
        {
            controller.ViewBag.ClubClassOnlineLinks = new SelectList(get(), ClubClassOnlineLinksModel.COL_Id.Name, ClubClassOnlineLinksModel.COL_Name.Name);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, string Name, Guid ClubClasses_Id)
        {
            return db.Database.SqlQuery<ClubClassOnlineLinksModel>(@"
                        SELECT ClubClassOnlineLinks.*
                        FROM ClubClassOnlineLinks
                        WHERE 1=1 
							AND (@Id IS NULL OR (ClubClassOnlineLinks.Id <> @Id))
                            AND ClubClassOnlineLinks.Name = @Name 
                            AND ClubClassOnlineLinks.ClubClasses_Id = @ClubClasses_Id
                    ",
                    DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Name.Name, Name),
                    DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_ClubClasses_Id.Name, ClubClasses_Id)
                ).Count() > 0;
        }

        public static List<ClubClassOnlineLinksModel> get(DateTime? DisplayStartDate, DateTime? DisplayEndDate) { return get(null, 1, null, null, DisplayStartDate, DisplayEndDate); }
        public static List<ClubClassOnlineLinksModel> get(int? FILTER_Active) { return get(null, FILTER_Active, null, null, null, null); }
        public List<ClubClassOnlineLinksModel> get(string FILTER_Keyword, int? FILTER_Active, Guid? FILTER_ClubClasses_Id) { return get(null, FILTER_Active, FILTER_Keyword, FILTER_ClubClasses_Id, null, null); }
        public static ClubClassOnlineLinksModel get(Guid Id) { return get(Id, null, null, null, null, null).FirstOrDefault(); }
        public static List<ClubClassOnlineLinksModel> get() { return get(null, null, null, null, null, null); }
        public static List<ClubClassOnlineLinksModel> get(Guid? Id, int? FILTER_Active, string FILTER_Keyword, Guid? FILTER_ClubClasses_Id, DateTime? DisplayStartDate, DateTime? DisplayEndDate)
        {
            string sql = string.Format(@"
                        SELECT ClubClassOnlineLinks.*,
                            ClubClasses.Name AS ClubClasses_Name,
                            DATEADD(day,ClubClasses.PeriodAdjustmentDayCount+(ClubClassOnlineLinks.WeekNo-1)*7,ISNULL(ClubClasses.PeriodStartDate,'1/1/1970')) AS DisplayStartDate,
                            DATEADD(day,ClubClasses.PeriodAdjustmentDayCount+(ClubClassOnlineLinks.WeekNo-1)*7+ClubClassOnlineLinks.DurationDays,ISNULL(ClubClasses.PeriodStartDate,'1/1/1970')) AS DisplayEndDate
                        FROM ClubClassOnlineLinks
                            LEFT JOIN ClubClasses ON ClubClasses.Id = ClubClassOnlineLinks.ClubClasses_Id
                        WHERE 1=1
							AND (@Id IS NULL OR ClubClassOnlineLinks.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Active IS NULL OR ClubClassOnlineLinks.Active = @Active)
    							AND (@FILTER_Keyword IS NULL OR (ClubClassOnlineLinks.Name LIKE '%'+@FILTER_Keyword+'%'))
                                AND (@ClubClasses_Id IS NULL OR ClubClassOnlineLinks.ClubClasses_Id = @ClubClasses_Id)
                                AND (@DisplayStartDate IS NULL OR @DisplayEndDate IS NULL OR (
                                    (
                                        @DisplayStartDate >= DATEADD(day,ClubClasses.PeriodAdjustmentDayCount+(ClubClassOnlineLinks.WeekNo-1)*7,ISNULL(ClubClasses.PeriodStartDate,'1/1/1970'))
                                        AND @DisplayStartDate < DATEADD(day,ClubClasses.PeriodAdjustmentDayCount+(ClubClassOnlineLinks.WeekNo-1)*7+ClubClassOnlineLinks.DurationDays,ISNULL(ClubClasses.PeriodStartDate,'1/1/1970'))
                                    ) OR (
                                        @DisplayStartDate <= DATEADD(day,ClubClasses.PeriodAdjustmentDayCount+(ClubClassOnlineLinks.WeekNo-1)*7,ISNULL(ClubClasses.PeriodStartDate,'1/1/1970'))
                                        AND @DisplayEndDate >= DATEADD(day,ClubClasses.PeriodAdjustmentDayCount+(ClubClassOnlineLinks.WeekNo-1)*7,ISNULL(ClubClasses.PeriodStartDate,'1/1/1970'))
                                    ) OR (
                                        @DisplayEndDate > DATEADD(day,ClubClasses.PeriodAdjustmentDayCount+(ClubClassOnlineLinks.WeekNo-1)*7,ISNULL(ClubClasses.PeriodStartDate,'1/1/1970'))
                                        AND @DisplayEndDate <= DATEADD(day,ClubClasses.PeriodAdjustmentDayCount+(ClubClassOnlineLinks.WeekNo-1)*7+ClubClassOnlineLinks.DurationDays,ISNULL(ClubClasses.PeriodStartDate,'1/1/1970'))
                                    )
                                ))
                            ))
						ORDER BY ClubClasses.Name ASC, ClubClassOnlineLinks.WeekNo ASC, ClubClassOnlineLinks.Name ASC
                    ");

            return new DBContext().Database.SqlQuery<ClubClassOnlineLinksModel>(sql,
                    DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Active.Name, FILTER_Active),
                    DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_ClubClasses_Id.Name, FILTER_ClubClasses_Id),
                    DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_DisplayStartDate.Name, DisplayStartDate),
                    DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_DisplayEndDate.Name, DisplayEndDate),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
                ).ToList();
        }

        public void add(ClubClassOnlineLinksModel model)
        {
            LIBWebMVC.WebDBConnection.Insert(db.Database, "ClubClassOnlineLinks",
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Name.Name, model.Name),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_OnlineLink.Name, model.OnlineLink),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_ClubClasses_Id.Name, model.ClubClasses_Id),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_WeekNo.Name, model.WeekNo),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_DurationDays.Name, model.DurationDays),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Notes.Name, model.Notes)
            );
            ActivityLogsController.AddCreateLog(db, Session, model.Id);
        }

        public void update(ClubClassOnlineLinksModel model, string log)
        {
            LIBWebMVC.WebDBConnection.Update(db.Database, "ClubClassOnlineLinks",
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Name.Name, model.Name),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_OnlineLink.Name, model.OnlineLink),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_ClubClasses_Id.Name, model.ClubClasses_Id),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_WeekNo.Name, model.WeekNo),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_DurationDays.Name, model.DurationDays),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(ClubClassOnlineLinksModel.COL_Notes.Name, model.Notes)
            );
            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
        }

        /******************************************************************************************************************************************************/
    }
}