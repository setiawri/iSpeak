using System;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LIBUtil;

namespace iSpeakWebApp
{
    public struct ModelMember
    {
        public string Name;
        public Guid Id;
        public string Display;
    }

    public class Helper
    {
        private static readonly DBContext db = new DBContext();

        /* PUBLIC PROPERTIES **********************************************************************************************************************************/

        public const string APP_VERSION = "v201211";
        public const string COMPANYNAME = "iSpeak";

        public const string IMAGEFOLDERURL = "/assets/img/";
        public const string IMAGEFOLDERPATH = "~"+ IMAGEFOLDERURL;
        public const string NOIMAGEFILE = "no-image.jpg";

        /* DATABASE INFORMATION *******************************************************************************************************************************/

        public const string DEVCOMPUTERNAME = "RQ-ASUS";
        private const string SERVERNAME_DEV = @".\SQLEXPRESS";
        private const string SERVERNAME_LIVE = "43.255.152.25";
        private const string DBNAME = "iSpeakWeb";
        private const string USERID = "ispeak";
        private const string PASSWORD = "1SpeakWell";

        /* METHODS ********************************************************************************************************************************************/

        public static string ConnectionString { get { return DBConnection.getWebConnectionString(Environment.MachineName == DEVCOMPUTERNAME ? SERVERNAME_DEV : SERVERNAME_LIVE, DBNAME, USERID, PASSWORD); } }

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

        public static DateTime setFilterViewBag(ControllerBase controller, int? year, int? month, DateTime? PayPeriod, string search, string periodChange, int? ActionType)
        {
            DateTime payPeriod;

            if (PayPeriod != null)
                payPeriod = (DateTime)PayPeriod;
            else if (month == null || year == null)
                payPeriod = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            else
                payPeriod = new DateTime((int)year, (int)month, 1, 0, 0, 0);

            if (periodChange == EnumActions.Previous.ToString())
                payPeriod = payPeriod.AddMonths(-1);
            else if (periodChange == EnumActions.Next.ToString())
                payPeriod = payPeriod.AddMonths(1);

            var ViewBag = controller.ViewBag;
            ViewBag.PayPeriodYear = Util.validateParameter(payPeriod.Year);
            ViewBag.PayPeriodMonth = Util.validateParameter(payPeriod.Month);
            ViewBag.PayPeriod = Util.validateParameter(payPeriod);
            ViewBag.Search = Util.validateParameter(search);
            ViewBag.ActionType = Util.validateParameter(ActionType);

            return payPeriod;
        }

        /******************************************************************************************************************************************************/
    }
}