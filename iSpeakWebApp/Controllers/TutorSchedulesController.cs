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

        /* SEARCH *********************************************************************************************************************************************/

        public ActionResult Search(string FILTER_Keyword, int? FILTER_Active)
        {
            if (!UserAccountsController.getUserAccess(Session).TutorSchedules_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Active);
            TutorSchedulesModel model = new TutorSchedulesModel();
            model.StartTime = new DateTime(1970, 1, 1, 7, 0, 0);
            model.EndTime = new DateTime(1970, 1, 1, 21, 0, 0);
            LanguagesController.setDropDownListViewBag(this);
            return View(model);
        }

        public JsonResult GetSchedules(Guid? Tutor_UserAccounts_Id, Guid? Languages_Id, DayOfWeekEnum DayOfWeek, DateTime StartTime, DateTime EndTime)
        {
            StartTime = standardizeTime(StartTime);
            EndTime = standardizeTime(EndTime);

            string content = string.Format(@"
                    <div class='table-responsive'>
                        <table class='table table-striped table-condensed'>
                            <thead>
                                <tr>
                                    <th class='text-center'>Tutor</th>
                ");

            //create columns
            DateTime counter = StartTime;
            List<DateTime> columns = new List<DateTime>();
            while(counter <= EndTime)
            {
                content += string.Format("<th class='text-center px-0'>{0:HH:mm}</th>", counter);
                columns.Add(counter);
                counter = counter.AddMinutes(30);
            }

            content += string.Format(@"
                                </tr>
                            </thead>
                            <tbody>
                ");

            //generate table content
            Dictionary<Guid, List<string>> dictionary = new Dictionary<Guid, List<string>>();
            List<TutorSchedulesModel> schedules = get(Tutor_UserAccounts_Id, Languages_Id, DayOfWeek, StartTime, EndTime);
            foreach (TutorSchedulesModel schedule in schedules)
            {
                List<string> row = new List<string>();
                if (dictionary.ContainsKey(schedule.Tutor_UserAccounts_Id))
                    row = dictionary[schedule.Tutor_UserAccounts_Id];
                else
                {
                    row.Add(string.Format("<tr><td class='py-1 py-2'><a target='_blank' href='{0}'>{1}</a></td>",
                                Url.Action("Index", "UserAccounts", new { FILTER_Keyword = schedule.Tutor_UserAccounts_No }),
                                schedule.Tutor_UserAccounts_Name));
                    foreach (DateTime column in columns)
                        row.Add("<td></td>");
                    dictionary.Add(schedule.Tutor_UserAccounts_Id, row);
                } 

                //add cell content
                for (int i=0; i<columns.Count; i++)
                {
                    if(columns[i] >= schedule.StartTime && columns[i] <= schedule.EndTime)
                    {
                        row[i+1] = string.Format("<td class='px-0 py-1'><a target='_blank' href='{0}'><span class='btn btn-success d-block py-2' style='border-radius: 0 !important;'></span></a></td>",
                                Url.Action("Create", "TutorStudentSchedules", new { DayOfWeek = (int)DayOfWeek, StartTime = string.Format("{0:HH_mm}", StartTime), Id = schedule.Tutor_UserAccounts_Id }));
                    }
                }

            }

            //create rows
            foreach (KeyValuePair<Guid, List<string>> row in dictionary)
            {
                foreach (string cell in row.Value)
                    content += cell;

                content += "</tr>";
            }

            content += string.Format(@"
                            </tbody>
                        </table>
                    </div>
                ");

            return Json(new { content = content }, JsonRequestBehavior.AllowGet);
        }

        /* METHODS ********************************************************************************************************************************************/

        public static void standardizeTime(TutorSchedulesModel model)
        {
            model.StartTime = standardizeTime(model.StartTime);
            model.EndTime = standardizeTime(model.EndTime);
        }

        public static DateTime standardizeTime(DateTime datetime)
        {
            return new DateTime(1970, 1, 1, datetime.Hour, datetime.Minute, 0);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(Guid? Id, Guid Tutor_UserAccounts_Id, DayOfWeekEnum DayOfWeek, DateTime StartTime, DateTime EndTime)
        {
            return db.Database.SqlQuery<TutorSchedulesModel>(@"
                        SELECT TutorSchedules.*
                        FROM TutorSchedules
                        WHERE 1=1 
							AND (@Id IS NULL OR TutorSchedules.Id <> @Id)
                            AND TutorSchedules.Tutor_UserAccounts_Id = @Tutor_UserAccounts_Id 
                            AND TutorSchedules.[DayOfWeek] = @DayOfWeek
                            AND (@StartTime IS NULL OR (@StartTime >= TutorSchedules.StartTime OR @StartTime < TutorSchedules.EndTime))
                            AND (@EndTime IS NULL OR (@EndTime > TutorSchedules.StartTime OR @EndTime <= TutorSchedules.EndTime))
                    ",
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_Tutor_UserAccounts_Id.Name, Tutor_UserAccounts_Id),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_DayOfWeek.Name, DayOfWeek),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_StartTime.Name, StartTime),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_EndTime.Name, EndTime)
                ).Count() > 0;
        }

        public List<TutorSchedulesModel> get(Guid? Tutor_UserAccounts_Id, Guid? Languages_Id, DayOfWeekEnum? DayOfWeek, DateTime? StartTime, DateTime? EndTime) 
        { return get(null, Tutor_UserAccounts_Id, DayOfWeek, StartTime, EndTime, Languages_Id, 1, null); }
        public List<TutorSchedulesModel> get(string FILTER_Keyword, int? FILTER_Active) { return get(null, null, null, null, null, null, FILTER_Active, FILTER_Keyword); }
        public TutorSchedulesModel get(Guid Id) { return get(Id, null, null, null, null, null, null, null).FirstOrDefault(); }
        public static List<TutorSchedulesModel> get() { return get(null, null, null, null, null, null, null, null); }
        public static List<TutorSchedulesModel> get(Guid? Id, Guid? Tutor_UserAccounts_Id, DayOfWeekEnum? DayOfWeek, DateTime? StartTime, DateTime? EndTime, 
            Guid? Languages_Id, int? Active, string FILTER_Keyword)
        {
            List<TutorSchedulesModel> models = new DBContext().Database.SqlQuery<TutorSchedulesModel>(@"
                        SELECT TutorSchedules.*,
                            UserAccounts.Fullname AS Tutor_UserAccounts_Name,
                            UserAccounts.No AS Tutor_UserAccounts_No,
                            UserAccounts.Interest AS UserAccounts_Interest
                        FROM TutorSchedules
                            LEFT JOIN UserAccounts ON UserAccounts.Id = TutorSchedules.Tutor_UserAccounts_Id
                        WHERE 1=1
							AND (@Id IS NULL OR TutorSchedules.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@Active IS NULL OR TutorSchedules.Active = @Active)
                                AND (@Tutor_UserAccounts_Id IS NULL OR TutorSchedules.Tutor_UserAccounts_Id = @Tutor_UserAccounts_Id)
                                AND (@DayOfWeek IS NULL OR TutorSchedules.[DayOfWeek] = @DayOfWeek)
                                AND ((@StartTime IS NULL OR (@StartTime >= TutorSchedules.StartTime OR @StartTime <= TutorSchedules.EndTime))
                                    OR (@EndTime IS NULL OR (@EndTime >= TutorSchedules.StartTime OR @EndTime <= TutorSchedules.EndTime))
                                    )
                                AND (@Languages_Id IS NULL OR UserAccounts.Interest LIKE '%'+ convert(nvarchar(50), @Languages_Id) + '%')
    							AND (@FILTER_Keyword IS NULL OR (
                                    UserAccounts.Fullname LIKE '%'+@FILTER_Keyword+'%'
                                    OR TutorSchedules.DayOfWeek LIKE '%'+@FILTER_Keyword+'%'
                                ))
                            ))
						ORDER BY UserAccounts.Fullname ASC, TutorSchedules.DayOfWeek ASC, TutorSchedules.StartTime ASC, TutorSchedules.EndTime ASC
                    ",
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_Active.Name, Active),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_Tutor_UserAccounts_Id.Name, Tutor_UserAccounts_Id),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_DayOfWeek.Name, DayOfWeek),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_StartTime.Name, StartTime),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_EndTime.Name, EndTime),
                    DBConnection.getSqlParameter("Languages_Id", Languages_Id),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
                ).ToList();

            foreach (TutorSchedulesModel model in models)
            {
                if (!string.IsNullOrEmpty(model.UserAccounts_Interest)) model.UserAccounts_Interest_List = model.UserAccounts_Interest.Split(',').ToList();
            }

            return models;
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