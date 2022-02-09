using System;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using iSpeakWebApp.Controllers;
using iSpeakWebApp.Models;
using LIBUtil;
using LIBWebMVC;

namespace iSpeakWebApp
{
    public struct ModelMember
    {
        public string Name;
        public Guid Id;
        public string Display;
        public string LogDisplay;
    }

    public class Select2Pagination
    {
        public class Select2Results
        {
            public Guid id { get; set; }
            public string text { get; set; }
            public string info1 { get; set; }
            public string info2 { get; set; }
            public string info3 { get; set; }
        }

        public class Select2Page
        {
            public bool more { get; set; }
        }
    }

    public class Helper
    {
        private static readonly DBContext db = new DBContext();

        public static string[] badgeColors = {
            "badge-primary", "badge-secondary", "badge-info", "badge-success", "badge-dark", "badge-warning", "badge-danger", "badge-light",
            "badge-primary", "badge-secondary", "badge-info", "badge-success", "badge-dark", "badge-warning", "badge-danger", "badge-light",
            "badge-primary", "badge-secondary", "badge-info", "badge-success", "badge-dark", "badge-warning", "badge-danger", "badge-light"
        };

        /* PUBLIC PROPERTIES **********************************************************************************************************************************/

        public const string APP_VERSION = "v210125";
        public const string COMPANYNAME = "iSpeak Language Center";

        public const string IMAGEFOLDERURL = "/assets/img/";
        public const string IMAGEFOLDERPATH = "~"+ IMAGEFOLDERURL;
        public const string NOIMAGEFILE = "no-image.jpg";

        /* DATABASE INFORMATION *******************************************************************************************************************************/

        public const string DEVCOMPUTERNAME = "RQ";
        private const string SERVERNAME_DEV = @".";
        private const string SERVERNAME_LIVE = "43.255.152.25";
        private const string DBNAME = "iSpeakWeb";
        private const string USERID = "ispeak";
        private const string PASSWORD = "1SpeakWell";

        /* METHODS ********************************************************************************************************************************************/

        public static string ConnectionString { get {
                bool ConnectToLiveDB = Convert.ToBoolean(Util.getConfigVariable("ConnectToLiveDB"));
                string datasource = Environment.MachineName == DEVCOMPUTERNAME && !ConnectToLiveDB ? SERVERNAME_DEV : SERVERNAME_LIVE;
                return DBConnection.getWebConnectionString(datasource, DBNAME, USERID, PASSWORD); } }

        public static string getImageUrl(string imageName, HttpRequestBase Request, HttpServerUtilityBase Server)
        {
            string filename = NOIMAGEFILE;
            if (!string.IsNullOrEmpty(imageName))
            {
                string dir = Server.MapPath(IMAGEFOLDERPATH);
                string path = Path.Combine(dir, imageName);
                if (File.Exists(path))
                    filename = imageName;
            }

            return (Request.ApplicationPath + IMAGEFOLDERURL + filename).Replace("//", "/");
        }


        public static bool isActiveBranchAvailable(HttpSessionStateBase Session) { return Session[UserAccountsController.SESSION_ActiveBranches_Id] != null; }
        public static Guid getActiveBranchId(HttpSessionStateBase Session) 
        {
            return (Guid)Session[UserAccountsController.SESSION_ActiveBranches_Id];
        }

        public static DateTime setFilterViewBag(ControllerBase controller, int? year, int? month, DateTime? PayPeriod, string search, string periodChange, int? ActionType)
        {
            DateTime payPeriod;

            if (PayPeriod != null)
                payPeriod = (DateTime)PayPeriod;
            else if (month == null || year == null)
                payPeriod = new DateTime(getCurrentDateTime().Year, getCurrentDateTime().Month, 1, 0, 0, 0);
            else
                payPeriod = new DateTime((int)year, (int)month, 1, 0, 0, 0);

            if (periodChange == EnumActions.Previous.ToString())
                payPeriod = payPeriod.AddMonths(-1);
            else if (periodChange == EnumActions.Next.ToString())
                payPeriod = payPeriod.AddMonths(1);

            var ViewBag = controller.ViewBag;
            ViewBag.PayPeriodYear = UtilWebMVC.validateParameter(payPeriod.Year);
            ViewBag.PayPeriodMonth = UtilWebMVC.validateParameter(payPeriod.Month);
            ViewBag.PayPeriod = UtilWebMVC.validateParameter(payPeriod);
            ViewBag.Search = UtilWebMVC.validateParameter(search);
            ViewBag.ActionType = UtilWebMVC.validateParameter(ActionType);

            return payPeriod;
        }

        public static string append<T>(string log, string value, string delimiter)
        {
            value = getName<T>(value);

            return Util.append(log, value, delimiter);
        }

        public static string append(string log, object oldValue, object newValue, string format)
        {
            return Util.webAppendChange(log, oldValue, newValue, format);
        }

        public static string append<T>(string log, object oldValue, object newValue, string format)
        {
            if(oldValue != null) oldValue = getName<T>(oldValue);
            if (newValue != null) newValue = getName<T>(newValue);

            return Util.webAppendChange(log, oldValue, newValue, format);
        }

        public static string addLog(HttpSessionStateBase Session, string log, Guid reffId, object oldValue, object newValue, string format)
        {
            string newlog = string.Empty;
            newlog = Util.appendChange(newlog, oldValue, newValue, format);
            if (string.IsNullOrEmpty(newlog))
                return log;
            else
            {
                ActivityLogsController.Add(db, Session, reffId, newlog);
                db.SaveChanges();
                return Util.append(log, string.Format("UPDATE: {0} to {1}", oldValue, newValue), Environment.NewLine + Environment.NewLine);
            }
        }

        public static string addLogForList<T>(string log, List<string> oldValue, List<string> newValue, string format)
        {
            if (newValue != null)
                newValue = newValue.ConvertAll(d => d.ToUpper());

            if (oldValue != null)
                oldValue = oldValue.ConvertAll(d => d.ToUpper());

            string addedlog = string.Empty;
            if (newValue != null)
            {
                foreach (string value in newValue)
                {
                    if (oldValue != null && oldValue.Contains(value))
                        oldValue.Remove(value);
                    else
                        addedlog = append<T>(addedlog, value, ", ");
                }
            }
            if (!string.IsNullOrEmpty(addedlog)) addedlog = Environment.NewLine + "Added: " + addedlog;

            string removedlog = string.Empty;
            if (oldValue != null)
            {
                foreach (string value in oldValue)
                    removedlog = append<T>(removedlog, value, ", ");
            }

            if (!string.IsNullOrEmpty(removedlog)) removedlog = Environment.NewLine + "Removed: " + removedlog;

            if (!string.IsNullOrEmpty(removedlog) || !string.IsNullOrEmpty(addedlog))
            {
                string newlog = string.Format(format, removedlog + addedlog);
                return Util.append(log, newlog, Environment.NewLine + Environment.NewLine);
            }
            else
                return log;
        }

        public static string getName<T>(object value)
        {
            string id = value.ToString().ToLower();
            if (typeof(T) == typeof(UserAccountRolesModel))
                return db.UserAccountRoles.Where(x => x.Id.ToString().ToLower() == id).FirstOrDefault().Name;
            else if (typeof(T) == typeof(UserAccountsModel))
                return new UserAccountsController().get((Guid)value).Fullname;
            else if (typeof(T) == typeof(BranchesModel))
                return db.Branches.Where(x => x.Id.ToString().ToLower() == id).FirstOrDefault().Name;
            else if (typeof(T) == typeof(PromotionEventsModel))
                return db.PromotionEvents.Where(x => x.Id.ToString().ToLower() == id).FirstOrDefault().Name;
            else if (typeof(T) == typeof(LanguagesModel))
                return db.Languages.Where(x => x.Id.ToString().ToLower() == id).FirstOrDefault().Name;
            else if (typeof(T) == typeof(SaleInvoiceItemsModel))
                return SaleInvoiceItemsController.get_by_IdList(id).FirstOrDefault().SaleInvoices_No;
            else
                return null;
        }

        public static DateTime getCurrentDateTime()
        {
            return DateTime.UtcNow.AddHours(7); //Jakarta time
        }

        /******************************************************************************************************************************************************/
    }
}