using System;
using System.Web;
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
        private const string DEFAULT_EMPTY_CELL = "<td style='width:10px;'></td>";

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
            if (rss != null && !SettingsController.ShowOnlyOwnUserData(Session))
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
            model.StartTime = new DateTime(1970, 1, 1, 8, 0, 0);
            model.EndTime = new DateTime(1970, 1, 1, 20, 0, 0);
            LanguagesController.setDropDownListViewBag(this);
            return View(model);
        }

        public JsonResult Ajax_GetSchedules(Guid? Tutor_UserAccounts_Id, Guid? Languages_Id, DayOfWeekEnum DayOfWeek, DateTime StartTime, DateTime EndTime, int MinutesPerColumn)
        {
            StartTime = standardizeTime(StartTime);
            EndTime = standardizeTime(EndTime);

            //adjust minutes to multiplication of MinutesPerColumn
            StartTime = StartTime.AddMinutes(StartTime.Minute % MinutesPerColumn == 0 ? 0 : - 1 * (StartTime.Minute % MinutesPerColumn));            
            EndTime = EndTime.AddMinutes(EndTime.Minute % MinutesPerColumn == 0 ? 0 : MinutesPerColumn - (EndTime.Minute % MinutesPerColumn));

            string content = string.Format(@"
                    <div class='table-responsive mt-1'>
                        <table class='table table-bordered table-striped table-condensed'>
                            <thead>
                                <tr style='height:80px;'>
                                    <th class='text-center' style='width:200px'>Tutor</th>
                ");

            //create columns
            DateTime counter = StartTime;
            List<DateTime> columns = new List<DateTime>();
            while(counter < EndTime)
            {
                columns.Add(counter);
                counter = counter.AddMinutes(MinutesPerColumn);
            }

            //format header text
            int colspan = 60 / MinutesPerColumn;
            for (int i=0; i<columns.Count; i++)
            {
                if(i==0 && columns[i].Minute != 0)
                    content += string.Format("<th colspan='{0}' class='px-1 py-0' style='width:10px'></th>", (60 - columns[i].Minute) / MinutesPerColumn);
                else if (columns[i].Minute == 0)
                {
                    if (i + colspan > columns.Count)
                        colspan = columns.Count - i;
                    content += string.Format("<th colspan='{0}' class='px-1 py-0' style='width:10px'>{1:HH:mm}</th>", colspan, columns[i]);
                }
            }

            content += string.Format(@"
                                </tr>
                            </thead>
                            <tbody>
                ");

            //generate table content
            Dictionary<Guid, List<string>> tableContent = new Dictionary<Guid, List<string>>();
            Dictionary<Guid, List<string>> popovers = new Dictionary<Guid, List<string>>();
            List<TutorSchedulesModel> TutorSchedules = get(Tutor_UserAccounts_Id, Languages_Id, DayOfWeek, StartTime, EndTime);
            foreach (TutorSchedulesModel schedule in TutorSchedules)
            {
                List<string> row = initializeRow(ref tableContent, ref popovers, columns, schedule.Tutor_UserAccounts_Id, schedule.Tutor_UserAccounts_Name);
                List<string> popoverRow = popovers[schedule.Tutor_UserAccounts_Id];

                //add schedule
                for (int i=0; i<columns.Count; i++)
                {
                    if(columns[i] >= schedule.StartTime && columns[i] < schedule.EndTime)
                    {
                        row[i+1] = string.Format("<td class='px-0 py-1' style='width:10px'><a target='_blank' href='{0}'><span class='btn btn-success d-block py-2' style='border-radius: 0 !important;'></span></a></td>",
                                Url.Action("Create", "StudentSchedules", new { 
                                    DayOfWeek = (int)DayOfWeek, 
                                    StartTime = string.Format("{0:HH_mm}", columns[i]), 
                                    Id = schedule.Tutor_UserAccounts_Id, 
                                    Name = schedule.Tutor_UserAccounts_Name 
                                }));
                    }
                }
            }

            //add booked/expired slots
            List<StudentSchedulesModel> StudentSchedules = StudentSchedulesController.get(Session, Tutor_UserAccounts_Id, null, Languages_Id, DayOfWeek, StartTime, EndTime, null);
            string url;
            foreach (StudentSchedulesModel schedule in StudentSchedules)
            {
                List<string> row = initializeRow(ref tableContent, ref popovers, columns, schedule.Tutor_UserAccounts_Id, schedule.Tutor_UserAccounts_Name);
                List<string> popoverRow = popovers[schedule.Tutor_UserAccounts_Id];

                //add schedule
                for (int i = 0; i < columns.Count; i++)
                {
                    if (columns[i] >= schedule.StartTime && columns[i] < schedule.EndTime)
                    {
                        popoverRow[i+1] = Util.append(popoverRow[i+1], string.Format("[{0} hours] {1}", schedule.SessionHours_Remaining, schedule.Student_UserAccounts_Name), "<br>");
                        bool isMultipleSchedules = row[i + 1] != DEFAULT_EMPTY_CELL;
                        row[i + 1] = string.Format("<td class='px-0 py-1'><a target='_blank' href='{0}' class='btn {2} d-block py-2' style='border-radius: 0 !important;' data-toggle='popover' data-container='body' data-placement='bottom' data-content='{1}'></a></td>",
                                url = Url.Action("Index", "StudentSchedules", new { 
                                    FILTER_Keyword = isMultipleSchedules ? "" : schedule.Student_UserAccounts_Name, 
                                    FILTER_UserAccounts_Name = schedule.Tutor_UserAccounts_Name, 
                                    FILTER_Custom = StudentSchedulesController.generateFILTER_Custom(schedule.DayOfWeek, schedule.StartTime, schedule.EndTime)
                                }),
                                popoverRow[i + 1],
                                isMultipleSchedules ? "btn-primary" : (schedule.SessionHours_Remaining > 0 ? "btn-warning" : "btn-secondary")
                            );
                    }
                }
            }

            //create rows
            foreach (KeyValuePair<Guid, List<string>> row in tableContent)
            {
                foreach (string cell in row.Value)
                    content += cell;
            }

            content += string.Format(@"
                            </tbody>
                        </table>
                    </div>
                ");

            return Json(new { content = content }, JsonRequestBehavior.AllowGet);
        }

        public List<string> initializeRow(ref Dictionary<Guid, List<string>> tableContent, ref Dictionary<Guid, List<string>> popovers, List<DateTime> columns, Guid Tutor_UserAccounts_Id, string Tutor_UserAccounts_Name)
        {
            List<string> row = new List<string>();

            if (tableContent.ContainsKey(Tutor_UserAccounts_Id))
                row = tableContent[Tutor_UserAccounts_Id];
            else
            {
                List<string> popoverRow = new List<string>();

                //tutor column
                row.Add(string.Format("<tr><td class='py-1 px-1' style='min-width:200px;'><a target='_blank' href='{0}'>{1}</a></td>",
                        Url.Action("Index", "TutorSchedules", new { FILTER_Keyword = Tutor_UserAccounts_Name }),
                        Tutor_UserAccounts_Name));
                popoverRow.Add("");

                //time columns
                foreach (DateTime column in columns)
                {
                    row.Add(DEFAULT_EMPTY_CELL);
                    popoverRow.Add("");
                }

                //close row
                row.Add("</tr>");
                popoverRow.Add("");

                tableContent.Add(Tutor_UserAccounts_Id, row);
                popovers.Add(Tutor_UserAccounts_Id, popoverRow);
            }

            return row;
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
                            AND ((@StartTime >= TutorSchedules.StartTime AND @StartTime < TutorSchedules.EndTime)
                                OR (@EndTime > TutorSchedules.StartTime AND @EndTime <= TutorSchedules.EndTime)
                            )
                    ",
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_Tutor_UserAccounts_Id.Name, Tutor_UserAccounts_Id),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_DayOfWeek.Name, DayOfWeek),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_StartTime.Name, StartTime),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_EndTime.Name, EndTime)
                ).Count() > 0;
        }

        public List<TutorSchedulesModel> get(Guid? Tutor_UserAccounts_Id, Guid? Languages_Id, DayOfWeekEnum? DayOfWeek, DateTime? StartTime, DateTime? EndTime) 
        { return get(Session, null, Tutor_UserAccounts_Id, DayOfWeek, StartTime, EndTime, Languages_Id, 1, null); }
        public List<TutorSchedulesModel> get(string FILTER_Keyword, int? FILTER_Active) { return get(Session, null, null, null, null, null, null, FILTER_Active, FILTER_Keyword); }
        public TutorSchedulesModel get(Guid Id) { return get(Session, Id, null, null, null, null, null, null, null).FirstOrDefault(); }
        public List<TutorSchedulesModel> get() { return get(Session, null, null, null, null, null, null, null, null); }
        public static List<TutorSchedulesModel> get(HttpSessionStateBase Session, Guid? Id, Guid? Tutor_UserAccounts_Id, DayOfWeekEnum? DayOfWeek, DateTime? StartTime, DateTime? EndTime, 
            Guid? Languages_Id, int? Active, string FILTER_Keyword)
        {
            UserAccountsModel UserAccount = UserAccountsController.getUserAccount(Session);

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
                                AND (@ShowOnlyOwnUserData = 0 OR (TutorSchedules.Tutor_UserAccounts_Id = @UserAccounts_Id))
                                AND (@DayOfWeek IS NULL OR TutorSchedules.[DayOfWeek] = @DayOfWeek)
                                AND ((@StartTime IS NULL OR (@StartTime >= TutorSchedules.StartTime OR @StartTime <= TutorSchedules.EndTime))
                                    OR (@EndTime IS NULL OR (@EndTime >= TutorSchedules.StartTime OR @EndTime <= TutorSchedules.EndTime))
                                    )
                                AND (@Languages_Id IS NULL OR UserAccounts.Interest LIKE '%'+ convert(nvarchar(50), @Languages_Id) + '%')
    							AND (@FILTER_Keyword IS NULL OR (
                                    UserAccounts.Fullname LIKE '%'+@FILTER_Keyword+'%'
                                    OR TutorSchedules.DayOfWeek LIKE '%'+@FILTER_Keyword+'%'
                                ))
                                AND (@Branches_Id IS NULL OR UserAccounts.Branches LIKE '%'+ convert(nvarchar(50), @Branches_Id) + '%')
                            ))
						ORDER BY UserAccounts.Fullname ASC, TutorSchedules.DayOfWeek ASC, TutorSchedules.StartTime ASC, TutorSchedules.EndTime ASC
                    ",
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_Active.Name, Active),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_Tutor_UserAccounts_Id.Name, Tutor_UserAccounts_Id),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_DayOfWeek.Name, DayOfWeek),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_StartTime.Name, StartTime),
                    DBConnection.getSqlParameter(TutorSchedulesModel.COL_EndTime.Name, EndTime),
                    DBConnection.getSqlParameter("Branches_Id", Helper.getActiveBranchId(Session)),
                    DBConnection.getSqlParameter("Languages_Id", Languages_Id),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                    DBConnection.getSqlParameter("ShowOnlyOwnUserData", SettingsController.ShowOnlyOwnUserData(UserAccount.Roles_List)),
                    DBConnection.getSqlParameter("UserAccounts_Id", UserAccount.Id)
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