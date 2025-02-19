﻿using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    /*
     * StudentSchedules is filtered by student branch. Franchise filter is NOT applied here
     */

    public class StudentSchedulesController : Controller
    {
        private readonly DBContext db = new DBContext();
        public const string LOCATION_ONSITE = "ONSITE";
        public const string LOCATION_ONLINE = "ONLINE";

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, string FILTER_UserAccounts_Name, string FILTER_Custom)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_UserAccounts_Name = FILTER_UserAccounts_Name;
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: StudentSchedules
        public ActionResult Index(int? rss, string FILTER_Keyword, string FILTER_UserAccounts_Name, string FILTER_Custom)
        {
            if (!UserAccountsController.getUserAccess(Session).StudentSchedules_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_UserAccounts_Name, FILTER_Custom);
            if (rss != null && !SettingsController.ShowOnlyOwnUserData(Session))
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                return View(get(FILTER_Keyword, FILTER_UserAccounts_Name, FILTER_Custom));
            }
        }

        // POST: StudentSchedules
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, string FILTER_UserAccounts_Name, string FILTER_Custom)
        {
            setViewBag(FILTER_Keyword, FILTER_UserAccounts_Name, FILTER_Custom);
            return View(get(FILTER_Keyword, FILTER_UserAccounts_Name, FILTER_Custom));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: StudentSchedules/Create
        public ActionResult Create(string FILTER_Keyword, string FILTER_UserAccounts_Name, string FILTER_Custom, DayOfWeekEnum? DayOfWeek, string StartTime, Guid? Id, string Name)
        {
            if (!UserAccountsController.getUserAccess(Session).StudentSchedules_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_UserAccounts_Name, FILTER_Custom);
            StudentSchedulesModel model = new StudentSchedulesModel();
            if (Id != null)
            {
                model.Tutor_UserAccounts_Id = (Guid)Id;
                model.Tutor_UserAccounts_Name = Name;
            }
            if (DayOfWeek != null) model.DayOfWeek = (DayOfWeekEnum)DayOfWeek;
            if (!string.IsNullOrEmpty(StartTime))
            {
                model.StartTime = (DateTime)Util.standardizeTimeToIgnoreDate(StartTime);
                model.EndTime = model.StartTime.AddHours(1);
            }
            return View(model);
        }

        // POST: StudentSchedules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StudentSchedulesModel model, string FILTER_Keyword, string FILTER_UserAccounts_Name, string FILTER_Custom)
        {
            if (ModelState.IsValid)
            {
                parseLessonLocation(ref model);
                standardizeTimeToIgnoreDate(model);
                if (isExists(null, model.Tutor_UserAccounts_Id, model.Student_UserAccounts_Id, model.DayOfWeek, model.StartTime, model.EndTime))
                    ModelState.AddModelError(StudentSchedulesModel.COL_DayOfWeek.Name, "Ada bentrok waktu dengan jadwal murid atau tutor");
                else
                {
                    add(model);
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_UserAccounts_Name = FILTER_UserAccounts_Name, FILTER_Custom = FILTER_UserAccounts_Name });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_UserAccounts_Name, FILTER_Custom);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: StudentSchedules/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, string FILTER_UserAccounts_Name, string FILTER_Custom)
        {
            if (!UserAccountsController.getUserAccess(Session).StudentSchedules_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_UserAccounts_Name, FILTER_Custom);
            StudentSchedulesModel model = get((Guid)id);
            if (model.SessionHours_Remaining == 0)
            {
                model.SaleInvoiceItems_Id = Guid.Empty;
                ModelState.AddModelError(StudentSchedulesModel.COL_SaleInvoiceItems_Id.Name, "Paket sebelumnya sudah tidak memiliki sisa jam");
            }
            return View(model);
        }

        // POST: StudentSchedules/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StudentSchedulesModel modifiedModel, string FILTER_Keyword, string FILTER_UserAccounts_Name, string FILTER_Custom)
        {
            if (ModelState.IsValid)
            {
                standardizeTimeToIgnoreDate(modifiedModel);
                if (isExists(modifiedModel.Id, modifiedModel.Tutor_UserAccounts_Id, modifiedModel.Student_UserAccounts_Id, modifiedModel.DayOfWeek, modifiedModel.StartTime, modifiedModel.EndTime))
                    ModelState.AddModelError(StudentSchedulesModel.COL_DayOfWeek.Name, "Kombinasi sudah terdaftar atau ada jam bentrok");
                else
                {
                    StudentSchedulesModel originalModel = get(modifiedModel.Id);

                    parseLessonLocation(ref modifiedModel);

                    string log = string.Empty;
                    log = Helper.append<UserAccountsModel>(log, originalModel.Tutor_UserAccounts_Id, modifiedModel.Tutor_UserAccounts_Id, StudentSchedulesModel.COL_Tutor_UserAccounts_Id.LogDisplay);
                    log = Helper.append<UserAccountsModel>(log, originalModel.Student_UserAccounts_Id, modifiedModel.Student_UserAccounts_Id, StudentSchedulesModel.COL_Student_UserAccounts_Id.LogDisplay);
                    log = Helper.append<SaleInvoiceItemsModel>(log, originalModel.SaleInvoiceItems_Id, modifiedModel.SaleInvoiceItems_Id, StudentSchedulesModel.COL_SaleInvoiceItems_Id.LogDisplay);
                    log = Helper.append(log, originalModel.DayOfWeek, modifiedModel.DayOfWeek, StudentSchedulesModel.COL_DayOfWeek.LogDisplay);
                    log = Helper.append(log, originalModel.StartTime, modifiedModel.StartTime, StudentSchedulesModel.COL_StartTime.LogDisplay);
                    log = Helper.append(log, originalModel.EndTime, modifiedModel.EndTime, StudentSchedulesModel.COL_EndTime.LogDisplay);
                    log = Helper.append(log, originalModel.LessonLocation, modifiedModel.LessonLocation, StudentSchedulesModel.COL_LessonLocation.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, StudentSchedulesModel.COL_Active.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, StudentSchedulesModel.COL_Notes.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                        update(modifiedModel, log);

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword, FILTER_UserAccounts_Name = FILTER_UserAccounts_Name, FILTER_Custom = FILTER_UserAccounts_Name });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_UserAccounts_Name, FILTER_Custom);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public static void parseLessonLocation(ref StudentSchedulesModel model)
        {
            if (model.LessonLocationRadioButton == LOCATION_ONSITE)
                model.LessonLocation = LOCATION_ONSITE;
            else if (model.LessonLocationRadioButton == LOCATION_ONLINE)
                model.LessonLocation = LOCATION_ONLINE;
            else if (model.LessonLocationRadioButton == null)
                model.LessonLocation = null;
        }

        public static void standardizeTimeToIgnoreDate(StudentSchedulesModel model)
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

        public bool isExists(Guid? Id, Guid Tutor_UserAccounts_Id, Guid Student_UserAccounts_Id, DayOfWeekEnum DayOfWeek, DateTime StartTime, DateTime EndTime)
        {
            return db.Database.SqlQuery<StudentSchedulesModel>(@"
                        SELECT StudentSchedules.*
                        FROM StudentSchedules
                        WHERE 1=1 
							AND (@Id IS NULL OR StudentSchedules.Id <> @Id)
                            AND StudentSchedules.Tutor_UserAccounts_Id = @Tutor_UserAccounts_Id
                            AND StudentSchedules.Student_UserAccounts_Id = @Student_UserAccounts_Id
                            AND StudentSchedules.[DayOfWeek] = @DayOfWeek
                            AND ((@StartTime >= StudentSchedules.StartTime AND @StartTime < StudentSchedules.EndTime)
                                OR (@EndTime > StudentSchedules.StartTime AND @EndTime <= StudentSchedules.EndTime)
                            )
                    ",
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_Tutor_UserAccounts_Id.Name, Tutor_UserAccounts_Id),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_Student_UserAccounts_Id.Name, Student_UserAccounts_Id),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_DayOfWeek.Name, DayOfWeek),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_StartTime.Name, StartTime),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_EndTime.Name, EndTime)
                ).Count() > 0;
        }

        public List<StudentSchedulesModel> get(string FILTER_Keyword, string FILTER_UserAccounts_Name, string FILTER_Custom) 
        {
            DayOfWeekEnum? DayOfWeek = null;
            DateTime? StartTime = null;
            DateTime? EndTime = null;
            if (!string.IsNullOrEmpty(FILTER_Custom))
                parseFILTER_Custom(FILTER_Custom, ref DayOfWeek, ref StartTime, ref EndTime);

            return get(Session, null, null, null, DayOfWeek, StartTime, EndTime, null, null, FILTER_Keyword, FILTER_UserAccounts_Name); 
        }
        public static List<StudentSchedulesModel> get(HttpSessionStateBase Session, Guid? Tutor_UserAccounts_Id, Guid? Student_UserAccounts_Id, Guid? Languages_Id, DayOfWeekEnum? DayOfWeek, DateTime? StartTime, DateTime? EndTime, Guid? SaleInvoiceItems_Id)
        { return get(Session, null, Tutor_UserAccounts_Id, Student_UserAccounts_Id, DayOfWeek, StartTime, EndTime, SaleInvoiceItems_Id, Languages_Id, null, null); }
        public StudentSchedulesModel get(Guid Id) { return get(Session, Id, null, null, null, null, null, null, null, null, null).FirstOrDefault(); }
        public List<StudentSchedulesModel> get() { return get(Session, null, null, null, null, null, null, null, null, null, null); }
        public static List<StudentSchedulesModel> get(HttpSessionStateBase Session, Guid? Id, Guid? Tutor_UserAccounts_Id, Guid? Student_UserAccounts_Id, DayOfWeekEnum? DayOfWeek, DateTime? StartTime, DateTime? EndTime, Guid? SaleInvoiceItems_Id,
            Guid? Languages_Id, string FILTER_Keyword, string FILTER_UserAccounts_Name)
        {
            UserAccountsModel UserAccount = UserAccountsController.getUserAccount(Session);

            return new DBContext().Database.SqlQuery<StudentSchedulesModel>(@"
                        SELECT StudentSchedules.*,
                            StudentSchedules.LessonLocation AS LessonLocationRadioButton,
                            Tutor_UserAccounts.Fullname AS Tutor_UserAccounts_Name,
                            Tutor_UserAccounts.No AS Tutor_UserAccounts_No,
                            Student_UserAccounts.Fullname AS Student_UserAccounts_Name,
                            Student_UserAccounts.No AS Student_UserAccounts_No,
                            Student_UserAccounts.Branches AS Student_UserAccounts_Branches,
                            SaleInvoices.No AS SaleInvoices_No,
                            SaleInvoiceItems.Description AS SaleInvoiceItems_Description,
                            Languages.Name AS Languages_Name,
                            SaleInvoiceItems.SessionHours_Remaining AS SessionHours_Remaining
                        FROM StudentSchedules
                            LEFT JOIN UserAccounts Tutor_UserAccounts ON Tutor_UserAccounts.Id = StudentSchedules.Tutor_UserAccounts_Id
                            LEFT JOIN UserAccounts Student_UserAccounts ON Student_UserAccounts.Id = StudentSchedules.Student_UserAccounts_Id
                            LEFT JOIN SaleInvoiceItems ON SaleInvoiceItems.Id = StudentSchedules.SaleInvoiceItems_Id
                            LEFT JOIN LessonPackages ON LessonPackages.Id = SaleInvoiceItems.LessonPackages_Id
                            LEFT JOIN Languages ON Languages.Id = LessonPackages.Languages_Id
                            LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                        WHERE 1=1
							AND (@Id IS NULL OR StudentSchedules.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Tutor_UserAccounts_Id IS NULL OR StudentSchedules.Tutor_UserAccounts_Id = @Tutor_UserAccounts_Id)
                                AND (@Student_UserAccounts_Id IS NULL OR StudentSchedules.Student_UserAccounts_Id = @Student_UserAccounts_Id)
                                AND (@ShowOnlyOwnUserData = 0 OR (StudentSchedules.Student_UserAccounts_Id = @UserAccounts_Id OR StudentSchedules.Tutor_UserAccounts_Id = @UserAccounts_Id))
                                AND (@DayOfWeek IS NULL OR StudentSchedules.[DayOfWeek] = @DayOfWeek)
                                AND ((@StartTime IS NULL OR (@StartTime >= StudentSchedules.StartTime OR @StartTime <= StudentSchedules.EndTime))
                                    OR (@EndTime IS NULL OR (@EndTime >= StudentSchedules.StartTime OR @EndTime <= StudentSchedules.EndTime))
                                    )
                                AND (@Languages_Id IS NULL OR Languages.Id = @Languages_Id)
                                AND (@SaleInvoiceItems_Id IS NULL OR StudentSchedules.SaleInvoiceItems_Id = @SaleInvoiceItems_Id)
    							AND (@FILTER_Keyword IS NULL OR (
                                    Student_UserAccounts.Fullname LIKE '%'+@FILTER_Keyword+'%'
                                    OR SaleInvoices.No LIKE '%'+@FILTER_Keyword+'%'
                                    OR StudentSchedules.DayOfWeek LIKE '%'+@FILTER_Keyword+'%'
                                    OR StudentSchedules.LessonLocation LIKE '%'+@FILTER_Keyword+'%'
                                ))
                                AND (@FILTER_UserAccounts_Name IS NULL OR (                                    
                                    Tutor_UserAccounts.Fullname LIKE '%'+@FILTER_UserAccounts_Name+'%'
                                ))
                                AND (@Branches_Id IS NULL OR Student_UserAccounts.Branches LIKE '%'+ convert(nvarchar(50), @Branches_Id) + '%')
                            ))
						ORDER BY Student_UserAccounts.Fullname ASC, StudentSchedules.DayOfWeek ASC, StudentSchedules.StartTime ASC, StudentSchedules.EndTime ASC, Tutor_UserAccounts.Fullname ASC
                    ",
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_Tutor_UserAccounts_Id.Name, Tutor_UserAccounts_Id),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_Student_UserAccounts_Id.Name, Student_UserAccounts_Id),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_DayOfWeek.Name, DayOfWeek),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_StartTime.Name, StartTime),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_EndTime.Name, EndTime),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_SaleInvoiceItems_Id.Name, SaleInvoiceItems_Id),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_Languages_Id.Name, Languages_Id),
                    DBConnection.getSqlParameter("Branches_Id", Helper.getActiveBranchId(Session)),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                    DBConnection.getSqlParameter("FILTER_UserAccounts_Name", FILTER_UserAccounts_Name),
                    DBConnection.getSqlParameter("ShowOnlyOwnUserData", SettingsController.ShowOnlyOwnUserData(UserAccount.Roles_List)),
                    DBConnection.getSqlParameter("UserAccounts_Id", UserAccount.Id)
                ).ToList();
        }

        public void add(StudentSchedulesModel model)
        {
            LIBWebMVC.WebDBConnection.Insert(db.Database, "StudentSchedules",
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_Tutor_UserAccounts_Id.Name, model.Tutor_UserAccounts_Id),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_Student_UserAccounts_Id.Name, model.Student_UserAccounts_Id),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_DayOfWeek.Name, model.DayOfWeek),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_StartTime.Name, model.StartTime),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_EndTime.Name, model.EndTime),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_SaleInvoiceItems_Id.Name, model.SaleInvoiceItems_Id),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_LessonLocation.Name, model.LessonLocation),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_Notes.Name, model.Notes)
            );
            ActivityLogsController.AddCreateLog(db, Session, model.Id);
        }

        public void update(StudentSchedulesModel model, string log)
        {
            LIBWebMVC.WebDBConnection.Update(db.Database, "StudentSchedules",
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_Tutor_UserAccounts_Id.Name, model.Tutor_UserAccounts_Id),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_Student_UserAccounts_Id.Name, model.Student_UserAccounts_Id),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_SaleInvoiceItems_Id.Name, model.SaleInvoiceItems_Id),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_DayOfWeek.Name, model.DayOfWeek),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_StartTime.Name, model.StartTime),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_EndTime.Name, model.EndTime),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_LessonLocation.Name, model.LessonLocation),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_Notes.Name, model.Notes)
            );
            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
        }

        public void delete(Guid Id)
        {
            LIBWebMVC.WebDBConnection.Delete(db.Database, "StudentSchedules",
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_Id.Name, Id)
            );
        }

        /******************************************************************************************************************************************************/
    }
}