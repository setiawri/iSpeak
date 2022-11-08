using iSpeakWebApp.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LIBUtil;
using LIBWebMVC;

namespace iSpeakWebApp.Controllers
{
    public class ActivityLogsController : Controller
    {

        private readonly DBContext db = new DBContext();

        public static string editListStringFormat(string fieldName) { return fieldName + ": {0}"; }
        public static string editStringFormat(string fieldName) { return fieldName + ": '{0}' to '{1}'"; }
        public static string editStringFormat2(string fieldName) { return fieldName + ": '{0}'"; }
        public static string editStringFormat3(string fieldName) { return fieldName + ": '{1}'"; }
        public static string editIntFormat(string fieldName) { return fieldName + ": '{0:N0}' to '{1:N0}'"; }
        public static string editDateFormat(string fieldName) { return fieldName + ": '{0:dd/MM/yyyy}' to '{1:dd/MM/yyyy}'"; }
        public static string editTimeFormat(string fieldName) { return fieldName + ": '{0:HH:mm}' to '{1:HH:mm}'"; }
        public static string editDateTimeFormat(string fieldName) { return fieldName + ": '{0:dd/MM/yyyy HH:mm}' to '{1:dd/MM/yyyy HH:mm}'"; }
        public static string editDecimalFormat(string fieldName) { return fieldName + ": '{0:G29}' to '{1:G29}'"; }
        public static string editBooleanFormat(string fieldName) { return fieldName + ": {1}"; }

        /* INDEX **********************************************************************************************************************************************/

        public ActionResult Index(int? rss, string FILTER_Keyword, Guid? FILTER_ReferenceId,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (!UserAccountsController.getUserAccess(Session).Branches_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (UtilWebMVC.hasNoFilter(FILTER_Keyword, FILTER_ReferenceId, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo))
            {
                FILTER_chkDateFrom = true;
                FILTER_DateFrom = Util.getAsStartDate(Helper.getCurrentDateTime().AddMonths(-1));
            }

            setViewBag(FILTER_Keyword, FILTER_ReferenceId, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                return View(get(FILTER_Keyword, FILTER_ReferenceId, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo));
            }
        }

        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, Guid? FILTER_ReferenceId,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            setViewBag(FILTER_Keyword, FILTER_ReferenceId, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            return View(get(FILTER_Keyword, FILTER_ReferenceId, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo));
        }

        public void setViewBag(string FILTER_Keyword, Guid? FILTER_ReferenceId,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_ReferenceId = FILTER_ReferenceId;
            ViewBag.FILTER_chkDateFrom = FILTER_chkDateFrom;
            ViewBag.FILTER_DateFrom = FILTER_DateFrom;
            ViewBag.FILTER_chkDateTo = FILTER_chkDateTo;
            ViewBag.FILTER_DateTo = FILTER_DateTo;
            UserAccountsController.setDropDownListViewBag_ReferenceIds(this);
        }

        /* DISPLAY LOG ****************************************************************************************************************************************/

        public JsonResult Ajax_GetLog(Guid ReferenceId)
        {
            string message = @"<div class='table-responsive'>
                                    <table class='table table-striped table-bordered'>
                                        <thead>
                                            <tr>
                                                <th style='width:150px'>Timestamp</th>
                                                <th>Description</th>
                                                <th>User</th>
                                            </tr>
                                        </thead>
                                        <tbody>";

            foreach (ActivityLogsModel item in get(ReferenceId))
            {
                message += string.Format(@"
                            <tr>
                                <td class='align-top'>{0:dd/MM/yy HH:mm}</td>
                                <td class='align-top'>{1}</td>
                                <td class='align-top'>{2}</td>
                            </tr>
                    ", item.Timestamp, item.Description.Replace(Environment.NewLine, "<BR>"), item.UserAccounts_Fullname);
            }

            message += "</tbody></table></div>";
            return Json(new { content = message }, JsonRequestBehavior.AllowGet);
        }

        /* ADD ************************************************************************************************************************************************/

        public static void AddEditLog(DBContext db, HttpSessionStateBase Session, Guid ReferenceId, string log) { Add(db, Session, ReferenceId, "UPDATE:<BR>" + log); }
        public static void AddCreateLog(DBContext db, HttpSessionStateBase Session, Guid ReferenceId) { Add(db, Session, ReferenceId, "Created"); }
        public static void Add(DBContext db, HttpSessionStateBase Session, Guid ReferenceId, string description)
        {
            LIBWebMVC.WebDBConnection.Insert(db.Database, "ActivityLogs",
                DBConnection.getSqlParameter(ActivityLogsModel.COL_Id.Name, Guid.NewGuid()),
                DBConnection.getSqlParameter(ActivityLogsModel.COL_Timestamp.Name, Helper.getCurrentDateTime()),
                DBConnection.getSqlParameter(ActivityLogsModel.COL_ReferenceId.Name, ReferenceId),
                DBConnection.getSqlParameter(ActivityLogsModel.COL_Description.Name, description),
                DBConnection.getSqlParameter(ActivityLogsModel.COL_UserAccounts_Id.Name, (Guid)UserAccountsController.getUserId(Session))
            );
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public List<ActivityLogsModel> get(Guid? ReferenceId) { return get(null, ReferenceId, null, null, null, null); }
        public List<ActivityLogsModel> get(string FILTER_Keyword, Guid? ReferenceId,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (FILTER_chkDateFrom == null || !(bool)FILTER_chkDateFrom)
                FILTER_DateFrom = null;

            if (FILTER_chkDateTo == null || !(bool)FILTER_chkDateTo)
                FILTER_DateTo = null;

            List<ActivityLogsModel> models = db.Database.SqlQuery<ActivityLogsModel>(@"
                        SELECT ActivityLogs.Id,
                            ActivityLogs.Timestamp,
                            ActivityLogs.ReferenceId,
                            ActivityLogs.Description,
                            ActivityLogs.UserAccounts_Id,
                            UserAccounts.Fullname AS UserAccounts_Fullname,
                            UserAccounts.Roles AS Roles
                        FROM ActivityLogs
                            LEFT JOIN UserAccounts ON UserAccounts.Id = ActivityLogs.UserAccounts_Id
                        WHERE 1=1 
    						AND (@ReferenceId IS NULL OR ActivityLogs.ReferenceId = @ReferenceId)
    						AND (@FILTER_Keyword IS NULL OR (ActivityLogs.Description LIKE '%'+@FILTER_Keyword+'%'))
                            AND (@FILTER_DateFrom IS NULL OR ActivityLogs.Timestamp >= @FILTER_DateFrom)
                            AND (@FILTER_DateTo IS NULL OR ActivityLogs.Timestamp <= @FILTER_DateTo)
						ORDER BY ActivityLogs.Timestamp DESC
                    ",
                    DBConnection.getSqlParameter(ActivityLogsModel.COL_ReferenceId.Name, ReferenceId),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                    DBConnection.getSqlParameter("FILTER_DateFrom", FILTER_DateFrom),
                    DBConnection.getSqlParameter("FILTER_DateTo", Util.getAsEndDate(FILTER_DateTo))
                ).ToList();

            foreach (ActivityLogsModel model in models)
            {
                if (!string.IsNullOrEmpty(model.Roles)) model.Roles_List = model.Roles.Split(',').ToList();
            }

            return models;
        }

        public static void delete(Guid? ReferenceId, DateTime? StartDate, DateTime? EndDate)
        {
            new DBContext().Database.ExecuteSqlCommand(@"       
                IF @ReferenceId IS NOT NULL OR @StartDate IS NOT NULL OR @EndDate IS NOT NULL
	                DELETE ActivityLogs
	                WHERE 1=1
		                AND (@ReferenceId IS NULL OR ActivityLogs.ReferenceId = @ReferenceId)
		                AND (@StartDate IS NULL OR ActivityLogs.Timestamp >= @StartDate)
		                AND (@EndDate IS NULL OR ActivityLogs.Timestamp <= @EndDate)
            ",
                DBConnection.getSqlParameter(ActivityLogsModel.COL_ReferenceId.Name, ReferenceId),
                DBConnection.getSqlParameter("StartDate", StartDate),
                DBConnection.getSqlParameter("EndDate", EndDate)
            );
        }
        /******************************************************************************************************************************************************/
    }
}