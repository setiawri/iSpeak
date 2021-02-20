using iSpeakWebApp.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace iSpeakWebApp.Controllers
{
    public class ActivityLogsController : Controller
    {

        private readonly DBContext db = new DBContext();

        /* DISPLAY LOG ****************************************************************************************************************************************/

        public JsonResult GetLog(Guid ReffId)
        {
            string message = @"<div class='table-responsive'>
                                    <table class='table table-striped table-bordered'>
                                        <thead>
                                            <tr>
                                                <th>Timestamp</th>
                                                <th>Description</th>
                                                <th>Operator</th>
                                            </tr>
                                        </thead>
                                        <tbody>";

            var items = (from ActivityLog in db.ActivityLogs
                         join UserAccount in db.UserAccounts on ActivityLog.UserAccounts_Id equals UserAccount.Id
                         where ActivityLog.ReffId == ReffId
                         orderby ActivityLog.Timestamp descending
                         select new { ActivityLog, UserAccount }).ToList();

            foreach (var item in items)
            {
                message += string.Format(@"
                            <tr>
                                <td>{0:dd/MM/yyyy HH:mm}</td>
                                <td>{1}</td>
                                <td>{2}</td>
                            </tr>
                    ", item.ActivityLog.Timestamp, item.ActivityLog.Description.Replace(Environment.NewLine, "<BR>"), item.UserAccount.Fullname);
            }

            message += "</tbody></table></div>";
            return Json(new { content = message }, JsonRequestBehavior.AllowGet);
        }

        /* ADD ************************************************************************************************************************************************/

        public static void AddEditLog(DBContext db, HttpSessionStateBase Session, Guid reffId, string log) { Add(db, Session, reffId, "UPDATE:<BR>" + log); }
        public static void AddCreateLog(DBContext db, HttpSessionStateBase Session, Guid reffId) { Add(db, Session, reffId, "Created"); }
        public static void Add(DBContext db, HttpSessionStateBase Session, Guid reffId, string description)
        {
            db.ActivityLogs.Add(new ActivityLogsModel
            {
                Id = Guid.NewGuid(),
                ReffId = reffId,
                Timestamp = DateTime.Now,
                Description = description,
                UserAccounts_Id = (Guid)UserAccountsController.getUserId(Session)
            });
        }

        public static string editStringFormat(string fieldName) { return fieldName + ": '{0}' to '{1}'"; }
        public static string editIntFormat(string fieldName) { return fieldName + ": '{0:N0}' to '{1:N0}'"; }
        public static string editDateFormat(string fieldName) { return fieldName + ": '{0:dd/MM/yyyy}' to '{1:dd/MM/yyyy}'"; }
        public static string editDateTimeFormat(string fieldName) { return fieldName + ": '{0:dd/MM/yyyy HH:mm}' to '{1:dd/MM/yyyy HH:mm}'"; }
        public static string editDecimalFormat(string fieldName) { return fieldName + ": '{0:G29}' to '{1:G29}'"; }

        /******************************************************************************************************************************************************/
    }
}