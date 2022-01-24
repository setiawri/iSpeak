using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class StudentSchedulesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* FILTER *********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Active)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Active = FILTER_Active;
        }

        /* INDEX **********************************************************************************************************************************************/

        // GET: StudentSchedules
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).StudentSchedules_View)
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

        // POST: StudentSchedules
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Active)
        {
            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get(FILTER_Keyword, FILTER_Active));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: StudentSchedules/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Active, DayOfWeekEnum? DayOfWeek, string StartTime, Guid? Id, string Name)
        {
            if (!UserAccountsController.getUserAccess(Session).StudentSchedules_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active);
            StudentSchedulesModel model = new StudentSchedulesModel();
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

        // POST: StudentSchedules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StudentSchedulesModel model, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                standardizeTime(model);
                if (isExists(null, model.Tutor_UserAccounts_Id, model.Student_UserAccounts_Id, model.DayOfWeek, model.StartTime, model.EndTime))
                    ModelState.AddModelError(StudentSchedulesModel.COL_DayOfWeek.Name, "Ada bentrok waktu dengan jadwal murid atau tutor");
                else
                {
                    add(model);
                    return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: StudentSchedules/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).StudentSchedules_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(get((Guid)id));
        }

        // POST: StudentSchedules/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StudentSchedulesModel modifiedModel, string FILTER_Keyword, int? FILTER_Active)
        {
            if (ModelState.IsValid)
            {
                standardizeTime(modifiedModel);
                if (isExists(modifiedModel.Id, modifiedModel.Tutor_UserAccounts_Id, modifiedModel.Student_UserAccounts_Id, modifiedModel.DayOfWeek, modifiedModel.StartTime, modifiedModel.EndTime))
                    ModelState.AddModelError(StudentSchedulesModel.COL_DayOfWeek.Name, "Kombinasi sudah terdaftar atau ada jam bentrok");
                else
                {
                    StudentSchedulesModel originalModel = get(modifiedModel.Id);

                    string log = string.Empty;
                    log = Helper.append<UserAccountsModel>(log, originalModel.Tutor_UserAccounts_Id, modifiedModel.Tutor_UserAccounts_Id, StudentSchedulesModel.COL_Tutor_UserAccounts_Id.LogDisplay);
                    log = Helper.append<UserAccountsModel>(log, originalModel.Student_UserAccounts_Id, modifiedModel.Student_UserAccounts_Id, StudentSchedulesModel.COL_Student_UserAccounts_Id.LogDisplay);
                    log = Helper.append<SaleInvoiceItemsModel>(log, originalModel.SaleInvoiceItems_Id, modifiedModel.SaleInvoiceItems_Id, StudentSchedulesModel.COL_SaleInvoiceItems_Id.LogDisplay);
                    log = Helper.append(log, originalModel.DayOfWeek, modifiedModel.DayOfWeek, StudentSchedulesModel.COL_DayOfWeek.LogDisplay);
                    log = Helper.append(log, originalModel.StartTime, modifiedModel.StartTime, StudentSchedulesModel.COL_StartTime.LogDisplay);
                    log = Helper.append(log, originalModel.EndTime, modifiedModel.EndTime, StudentSchedulesModel.COL_EndTime.LogDisplay);
                    log = Helper.append(log, originalModel.Active, modifiedModel.Active, StudentSchedulesModel.COL_Active.LogDisplay);
                    log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, StudentSchedulesModel.COL_Notes.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                        update(modifiedModel, log);

                    return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword, FILTER_Active = FILTER_Active });
                }
            }

            setViewBag(FILTER_Keyword, FILTER_Active);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public static void standardizeTime(StudentSchedulesModel model)
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

        public static List<StudentSchedulesModel> get(HttpSessionStateBase Session, Guid? Tutor_UserAccounts_Id, Guid? Student_UserAccounts_Id, Guid? Languages_Id, DayOfWeekEnum? DayOfWeek, DateTime? StartTime, DateTime? EndTime, Guid? SaleInvoiceItems_Id) 
        { return get(Session, null, Tutor_UserAccounts_Id, Student_UserAccounts_Id, DayOfWeek, StartTime, EndTime, SaleInvoiceItems_Id, Languages_Id, 1, null); }
        public List<StudentSchedulesModel> get(string FILTER_Keyword, int? FILTER_Active) { return get(Session, null, null, null, null, null, null, null, null, FILTER_Active, FILTER_Keyword); }
        public StudentSchedulesModel get(Guid Id) { return get(Session, Id, null, null, null, null, null, null, null, null, null).FirstOrDefault(); }
        public List<StudentSchedulesModel> get() { return get(Session, null, null, null, null, null, null, null, null, null, null); }
        public static List<StudentSchedulesModel> get(HttpSessionStateBase Session, Guid? Id, Guid? Tutor_UserAccounts_Id, Guid? Student_UserAccounts_Id, DayOfWeekEnum? DayOfWeek, DateTime? StartTime, DateTime? EndTime, Guid? SaleInvoiceItems_Id,
            Guid? Languages_Id, int? Active, string FILTER_Keyword)
        {
            return new DBContext().Database.SqlQuery<StudentSchedulesModel>(@"
                        SELECT StudentSchedules.*,
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
                                (@Active IS NULL OR StudentSchedules.Active = @Active)
                                AND (@Tutor_UserAccounts_Id IS NULL OR StudentSchedules.Tutor_UserAccounts_Id = @Tutor_UserAccounts_Id)
                                AND (@Student_UserAccounts_Id IS NULL OR StudentSchedules.Student_UserAccounts_Id = @Student_UserAccounts_Id)
                                AND (@DayOfWeek IS NULL OR StudentSchedules.[DayOfWeek] = @DayOfWeek)
                                AND ((@StartTime IS NULL OR (@StartTime >= StudentSchedules.StartTime OR @StartTime <= StudentSchedules.EndTime))
                                    OR (@EndTime IS NULL OR (@EndTime >= StudentSchedules.StartTime OR @EndTime <= StudentSchedules.EndTime))
                                    )
                                AND (@Languages_Id IS NULL OR Languages.Id = @Languages_Id)
                                AND (@SaleInvoiceItems_Id IS NULL OR StudentSchedules.SaleInvoiceItems_Id = @SaleInvoiceItems_Id)
    							AND (@FILTER_Keyword IS NULL OR (
                                    Tutor_UserAccounts.Fullname LIKE '%'+@FILTER_Keyword+'%'
                                    OR Student_UserAccounts.Fullname LIKE '%'+@FILTER_Keyword+'%'
                                    OR SaleInvoices.No LIKE '%'+@FILTER_Keyword+'%'
                                    OR StudentSchedules.DayOfWeek LIKE '%'+@FILTER_Keyword+'%'
                                ))
                                AND (@Branches_Id IS NULL OR Student_UserAccounts.Branches LIKE '%'+ convert(nvarchar(50), @Branches_Id) + '%')
                            ))
						ORDER BY Student_UserAccounts.Fullname ASC, Tutor_UserAccounts.Fullname ASC, StudentSchedules.DayOfWeek ASC, StudentSchedules.StartTime ASC, StudentSchedules.EndTime ASC
                    ",
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_Active.Name, Active),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_Tutor_UserAccounts_Id.Name, Tutor_UserAccounts_Id),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_Student_UserAccounts_Id.Name, Student_UserAccounts_Id),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_DayOfWeek.Name, DayOfWeek),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_StartTime.Name, StartTime),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_EndTime.Name, EndTime),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_SaleInvoiceItems_Id.Name, SaleInvoiceItems_Id),
                    DBConnection.getSqlParameter(StudentSchedulesModel.COL_Languages_Id.Name, Languages_Id),
                    DBConnection.getSqlParameter("Branches_Id", Helper.getActiveBranchId(Session)),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
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
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_Active.Name, model.Active),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_Notes.Name, model.Notes)
            );
            ActivityLogsController.AddCreateLog(db, Session, model.Id);
            db.SaveChanges();
        }

        public void update(StudentSchedulesModel model, string log)
        {
            LIBWebMVC.WebDBConnection.Update(db.Database, "StudentSchedules",
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_Tutor_UserAccounts_Id.Name, model.Tutor_UserAccounts_Id),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_Student_UserAccounts_Id.Name, model.Student_UserAccounts_Id),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_DayOfWeek.Name, model.DayOfWeek),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_StartTime.Name, model.StartTime),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_EndTime.Name, model.EndTime),
                DBConnection.getSqlParameter(StudentSchedulesModel.COL_Notes.Name, model.Notes)
            );
            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
            db.SaveChanges();
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