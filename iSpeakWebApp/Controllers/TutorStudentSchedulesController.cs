using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class TutorStudentSchedulesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: TutorStudentSchedules
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).TutorStudentSchedules_View)
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

        // POST: TutorStudentSchedules
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active)
        {
            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get(FILTER_Keyword, FILTER_Active));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: TutorStudentSchedules/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active, DayOfWeekEnum? DayOfWeek, string StartTime, Guid? Id, string Name)
        {
            if (!UserAccountsController.getUserAccess(Session).TutorStudentSchedules_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active);
            TutorStudentSchedulesModel model = new TutorStudentSchedulesModel();
            if (Id != null)
            {
                model.Tutor_UserAccounts_Id = (Guid)Id;
                model.Tutor_UserAccounts_Name = Name;
            }
            if (DayOfWeek != null) model.DayOfWeek = (DayOfWeekEnum)DayOfWeek;
            if (!string.IsNullOrEmpty(StartTime))
            {
                model.StartTime = standardizeTime(StartTime);
                model.EndTime = model.StartTime.AddHours(1);
            }
            return View(model);
        }

        // POST: TutorStudentSchedules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TutorStudentSchedulesModel model, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                standardizeTime(model);
                if (isExists(model.Tutor_UserAccounts_Id, model.Student_UserAccounts_Id, model.DayOfWeek, model.StartTime, model.EndTime))
                    ModelState.AddModelError(TutorStudentSchedulesModel.COL_DayOfWeek.Name, "Ada bentrok waktu dengan jadwal murid atau tutor");
                else
                {
                    add(model);
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(model);
        }

        /* METHODS ********************************************************************************************************************************************/

        public static void standardizeTime(TutorStudentSchedulesModel model)
        {
            model.StartTime = standardizeTime(model.StartTime);
            model.EndTime = standardizeTime(model.EndTime);
        }

        public static DateTime standardizeTime(DateTime datetime)
        {
            return new DateTime(1970, 1, 1, datetime.Hour, datetime.Minute, 0);
        }

        public static DateTime standardizeTime(string time)
        {
            return new DateTime(1970, 1, 1, Convert.ToInt32(time.Split('_')[0]), Convert.ToInt32(time.Split('_')[1]), 0);
        }

        public JsonResult Ajax_Delete(Guid id)
        {
            delete(id);
            return Json(new { Message = "" });
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid Tutor_UserAccounts_Id, Guid Student_UserAccounts_Id, DayOfWeekEnum DayOfWeek, DateTime StartTime, DateTime EndTime)
        {
            return db.Database.SqlQuery<TutorStudentSchedulesModel>(@"
                        SELECT TutorStudentSchedules.*
                        FROM TutorStudentSchedules
                        WHERE 1=1 
                            AND TutorStudentSchedules.Student_UserAccounts_Id = @Student_UserAccounts_Id
                            AND TutorStudentSchedules.[DayOfWeek] = @DayOfWeek
                            AND ((@StartTime >= TutorStudentSchedules.StartTime AND @StartTime < TutorStudentSchedules.EndTime)
                                OR (@EndTime > TutorStudentSchedules.StartTime AND @EndTime <= TutorStudentSchedules.EndTime)
                            )
                    ",
                    DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_Tutor_UserAccounts_Id.Name, Tutor_UserAccounts_Id),
                    DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_Student_UserAccounts_Id.Name, Student_UserAccounts_Id),
                    DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_DayOfWeek.Name, DayOfWeek),
                    DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_StartTime.Name, StartTime),
                    DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_EndTime.Name, EndTime)
                ).Count() > 0;
        }

        public static List<TutorStudentSchedulesModel> get(Guid? Tutor_UserAccounts_Id, Guid? Student_UserAccounts_Id, Guid? Languages_Id, DayOfWeekEnum? DayOfWeek, DateTime? StartTime, DateTime? EndTime, Guid? SaleInvoiceItems_Id) 
        { return get(null, Tutor_UserAccounts_Id, Student_UserAccounts_Id, DayOfWeek, StartTime, EndTime, SaleInvoiceItems_Id, Languages_Id, 1, null); }
        public List<TutorStudentSchedulesModel> get(string FILTER_Keyword, int? FILTER_Active) { return get(null, null, null, null, null, null, null, null, FILTER_Active, FILTER_Keyword); }
        public TutorStudentSchedulesModel get(Guid Id) { return get(Id, null, null, null, null, null, null, null, null, null).FirstOrDefault(); }
        public static List<TutorStudentSchedulesModel> get() { return get(null, null, null, null, null, null, null, null, null, null); }
        public static List<TutorStudentSchedulesModel> get(Guid? Id, Guid? Tutor_UserAccounts_Id, Guid? Student_UserAccounts_Id, DayOfWeekEnum? DayOfWeek, DateTime? StartTime, DateTime? EndTime, Guid? SaleInvoiceItems_Id,
            Guid? Languages_Id, int? Active, string FILTER_Keyword)
        {
            return new DBContext().Database.SqlQuery<TutorStudentSchedulesModel>(@"
                        SELECT TutorStudentSchedules.*,
                            Tutor_UserAccounts.Fullname AS Tutor_UserAccounts_Name,
                            Tutor_UserAccounts.No AS Tutor_UserAccounts_No,
                            Student_UserAccounts.Fullname AS Student_UserAccounts_Name,
                            Student_UserAccounts.No AS Student_UserAccounts_No,
                            SaleInvoices.No AS SaleInvoices_No,
                            SaleInvoiceItems.Description AS SaleInvoiceItems_Description,
                            Languages.Name AS Languages_Name,
                            SaleInvoiceItems.SessionHours_Remaining AS SessionHours_Remaining
                        FROM TutorStudentSchedules
                            LEFT JOIN UserAccounts Tutor_UserAccounts ON Tutor_UserAccounts.Id = TutorStudentSchedules.Tutor_UserAccounts_Id
                            LEFT JOIN UserAccounts Student_UserAccounts ON Student_UserAccounts.Id = TutorStudentSchedules.Student_UserAccounts_Id
                            LEFT JOIN SaleInvoiceItems ON SaleInvoiceItems.Id = TutorStudentSchedules.SaleInvoiceItems_Id
                            LEFT JOIN LessonPackages ON LessonPackages.Id = SaleInvoiceItems.LessonPackages_Id
                            LEFT JOIN Languages ON Languages.Id = LessonPackages.Languages_Id
                            LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                        WHERE 1=1
							AND (@Id IS NULL OR TutorStudentSchedules.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Active IS NULL OR TutorStudentSchedules.Active = @Active)
                                AND (@Tutor_UserAccounts_Id IS NULL OR TutorStudentSchedules.Tutor_UserAccounts_Id = @Tutor_UserAccounts_Id)
                                AND (@Student_UserAccounts_Id IS NULL OR TutorStudentSchedules.Student_UserAccounts_Id = @Student_UserAccounts_Id)
                                AND (@DayOfWeek IS NULL OR TutorStudentSchedules.[DayOfWeek] = @DayOfWeek)
                                AND ((@StartTime IS NULL OR (@StartTime >= TutorStudentSchedules.StartTime OR @StartTime <= TutorStudentSchedules.EndTime))
                                    OR (@EndTime IS NULL OR (@EndTime >= TutorStudentSchedules.StartTime OR @EndTime <= TutorStudentSchedules.EndTime))
                                    )
                                AND (@Languages_Id IS NULL OR Languages.Id = @Languages_Id)
                                AND (@SaleInvoiceItems_Id IS NULL OR TutorStudentSchedules.SaleInvoiceItems_Id = @SaleInvoiceItems_Id)
    							AND (@FILTER_Keyword IS NULL OR (
                                    Tutor_UserAccounts.Fullname LIKE '%'+@FILTER_Keyword+'%'
                                    OR Student_UserAccounts.Fullname LIKE '%'+@FILTER_Keyword+'%'
                                    OR SaleInvoices.No LIKE '%'+@FILTER_Keyword+'%'
                                    OR TutorStudentSchedules.DayOfWeek LIKE '%'+@FILTER_Keyword+'%'
                                ))
                            ))
						ORDER BY Student_UserAccounts.Fullname ASC, Tutor_UserAccounts.Fullname ASC, TutorStudentSchedules.DayOfWeek ASC, TutorStudentSchedules.StartTime ASC, TutorStudentSchedules.EndTime ASC
                    ",
                    DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_Active.Name, Active),
                    DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_Tutor_UserAccounts_Id.Name, Tutor_UserAccounts_Id),
                    DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_Student_UserAccounts_Id.Name, Student_UserAccounts_Id),
                    DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_DayOfWeek.Name, DayOfWeek),
                    DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_StartTime.Name, StartTime),
                    DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_EndTime.Name, EndTime),
                    DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_SaleInvoiceItems_Id.Name, SaleInvoiceItems_Id),
                    DBConnection.getSqlParameter("Languages_Id", Languages_Id),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
                ).ToList();
        }

        public void add(TutorStudentSchedulesModel model)
        {
            LIBWebMVC.WebDBConnection.Insert(db.Database, "TutorStudentSchedules",
                DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_Tutor_UserAccounts_Id.Name, model.Tutor_UserAccounts_Id),
                DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_Student_UserAccounts_Id.Name, model.Student_UserAccounts_Id),
                DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_DayOfWeek.Name, model.DayOfWeek),
                DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_StartTime.Name, model.StartTime),
                DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_EndTime.Name, model.EndTime),
                DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_SaleInvoiceItems_Id.Name, model.SaleInvoiceItems_Id),
                DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_Notes.Name, model.Notes)
            );
            ActivityLogsController.AddCreateLog(db, Session, model.Id);
            db.SaveChanges();
        }

        public void delete(Guid Id)
        {
            LIBWebMVC.WebDBConnection.Delete(db.Database, "TutorStudentSchedules",
                DBConnection.getSqlParameter(TutorStudentSchedulesModel.COL_Id.Name, Id)
            );
        }

        /******************************************************************************************************************************************************/
    }
}