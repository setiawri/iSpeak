using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;
using LIBWebMVC;

namespace iSpeakWebApp.Controllers
{
    public class PayrollsController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: Payrolls
        public ActionResult Index(int? rss, DateTime? FILTER_DatePeriod)
        {
            if (!UserAccountsController.getUserAccess(Session).PayrollPayments_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_DatePeriod);
            ViewBag.RemoveDatatablesStateSave = rss;
            return View();
        }

        // POST: Payrolls
        [HttpPost]
        public ActionResult Index(DateTime? FILTER_DatePeriod)
        {
            List<PayrollsModel> models = null;
            setViewBag(FILTER_DatePeriod);
            if (FILTER_DatePeriod != null)
            {
                models = get(Util.getAsStartDate(FILTER_DatePeriod).Value, Util.getLastDayOfSelectedMonth(FILTER_DatePeriod.Value).Value);
            }

            return View(models);
        }

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(DateTime? FILTER_DatePeriod)
        {
            ViewBag.FILTER_DatePeriod = FILTER_DatePeriod ?? Util.getFirstDayOfSelectedMonth(DateTime.Now);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        private List<PayrollsModel> get(DateTime StartDate, DateTime EndDate)
        {
            List<PayrollsModel> models = db.Database.SqlQuery<PayrollsModel>(@"
                        SELECT Payrolls.*,
                            '' AS Tutor_UserAccounts_Id,
							ISNULL(Due.Amount,0) AS DueAmount,
							UserAccounts.Fullname AS Tutor_UserAccounts_FullName
						FROM (
								SELECT UserAccounts_Id_TEMP AS Tutor_UserAccounts_Id_TEMP,
									SUM(Hour) AS TotalHours,
									SUM(Amount) AS PayableAmount
								FROM PayrollPaymentItems
								WHERE PayrollPaymentItems.Timestamp >= @StartDate AND PayrollPaymentItems.Timestamp <= @EndDate
                                    AND PayrollPaymentItems.Branches_Id = @Branches_Id
								GROUP BY UserAccounts_Id_TEMP						
							) Payrolls
							LEFT JOIN UserAccounts ON UserAccounts.Id = Payrolls.Tutor_UserAccounts_Id_TEMP
							LEFT JOIN (
									SELECT UserAccounts_Id_TEMP AS Tutor_UserAccounts_Id_TEMP,
										SUM(Amount) AS Amount
									FROM PayrollPaymentItems
									WHERE PayrollPaymentItems.Timestamp >= @StartDate AND PayrollPaymentItems.Timestamp <= @EndDate
                                        AND PayrollPaymentItems.Branches_Id = @Branches_Id
										AND PayrollPayments_Id IS NULL
									GROUP BY UserAccounts_Id_TEMP						
								) Due ON Due.Tutor_UserAccounts_Id_TEMP = Payrolls.Tutor_UserAccounts_Id_TEMP
						ORDER BY UserAccounts.Fullname ASC
                    ",
                    DBConnection.getSqlParameter("Branches_Id", Helper.getActiveBranchId(Session)),
                    DBConnection.getSqlParameter("StartDate", StartDate),
                    DBConnection.getSqlParameter("EndDate", EndDate)
                ).ToList();

            return models;
        }

        /******************************************************************************************************************************************************/
    }
}