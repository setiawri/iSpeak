using System;
using System.Data;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;

using Newtonsoft.Json;

using LIBUtil;
using LIBWebMVC;

namespace iSpeakWebApp.Controllers
{
    public class ChartData
    {
        public string[] Labels { get; set; }
        public string[] DatasetLabels { get; set; }
        public List<decimal[]> DatasetDatas { get; set; }
    }

    public class ReportsController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        public ActionResult IncomeStatement(int? rss, int? FILTER_Approved,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (!UserAccountsController.getUserAccess(Session).PayrollPayments_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                FILTER_chkDateFrom = true;
                FILTER_DateFrom = new DateTime(DateTime.Now.AddYears(-1).Year, 1, 1);
                FILTER_chkDateTo = false;
                setViewBag(FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
                List<IncomeStatementModel> models = get(Session, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
                ViewBag.Data = compileChartData(models);
                return View(models);
            }
        }

        [HttpPost]
        public ActionResult IncomeStatement(bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            setViewBag(FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);

            List<IncomeStatementModel> models = get(Session, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            ViewBag.Data = compileChartData(models);
            return View(models);
        }

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            ViewBag.FILTER_chkDateFrom = FILTER_chkDateFrom;
            ViewBag.FILTER_DateFrom = FILTER_DateFrom;
            ViewBag.FILTER_chkDateTo = FILTER_chkDateTo;
            ViewBag.FILTER_DateTo = FILTER_DateTo;
        }

        public string compileChartData(List<IncomeStatementModel> models)
        {
            const string MONTHCOLUMN = "Month";

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn(MONTHCOLUMN, typeof(string)));
            foreach (IncomeStatementModel model in models)
                if (!dt.Columns.Contains(model.Year))
                    dt.Columns.Add(new DataColumn(model.Year, typeof(decimal)));

            string[] monthNames = System.Globalization.DateTimeFormatInfo.CurrentInfo.MonthNames;
            DataRow row;
            for(int i = 0; i < 12; i++)
            {
                row = dt.NewRow();
                row[MONTHCOLUMN] = monthNames[i];
                foreach (IncomeStatementModel item in models.Where(x => x.Month == i).ToList())
                    row[item.Year] = item.Revenue;
                foreach (DataColumn column in dt.Columns)
                    if (row[column] == DBNull.Value)
                        row[column] = 0;
                dt.Rows.Add(row);
            }

            ChartData chartData = new ChartData();

            string[] Labels = (dt.AsEnumerable().Select(p => p.Field<string>(MONTHCOLUMN))).Distinct().ToArray();
            chartData.Labels = Labels;

            List<string> datasetLabels = new List<string>();
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                datasetLabels.Add(dt.Columns[i].ColumnName);
            }
            chartData.DatasetLabels = datasetLabels.ToArray();

            List<decimal[]> datasetDatas = new List<decimal[]>();
            for (int i = 0; i < chartData.DatasetLabels.Length; i++)
            {
                List<decimal> data = new List<decimal>();
                for (int j = 0; j < Labels.Length; j++)
                {
                    decimal amount = (dt.AsEnumerable().Where(p => p.Field<string>(MONTHCOLUMN) == Labels[j])
                        .Select(p => p.Field<Decimal>(chartData.DatasetLabels[i]))).FirstOrDefault();
                    data.Add(amount);
                }
                datasetDatas.Add(data.ToArray());
            }

            chartData.DatasetDatas = datasetDatas;
            return JsonConvert.SerializeObject(chartData);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public static List<IncomeStatementModel> get(HttpSessionStateBase Session, bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (FILTER_chkDateFrom == null || !(bool)FILTER_chkDateFrom)
                FILTER_DateFrom = null;

            if (FILTER_chkDateTo == null || !(bool)FILTER_chkDateTo)
                FILTER_DateTo = null;

            return new DBContext().Database.SqlQuery<IncomeStatementModel>(@"
						SELECT	summarytable.MonthYear as MonthYear,
								SUBSTRING(summarytable.MonthYear, 0, 5) AS [Year],
								CAST(SUBSTRING(summarytable.MonthYear, 6, 2) AS INT) AS [Month],
								CAST(SUM(summarytable.Revenue) AS BIGINT) AS Revenue,
								CAST(SUM(summarytable.Expenses) AS DECIMAL) AS Expenses, 
								CAST(SUM(summarytable.Profit) AS DECIMAL) AS Profit,
								CAST((COALESCE(SUM(summarytable.Profit), 0) / IIF(SUM(summarytable.Expenses) = 0, 1, SUM(summarytable.Expenses)) * 100) AS DECIMAL) AS ProfitPercent
						FROM (
							SELECT 
								CAST(YEAR(SaleInvoices.Timestamp) AS VARCHAR(4)) + '-' + (SELECT RIGHT('00' + CAST(MONTH(SaleInvoices.Timestamp) AS VARCHAR(2)),2)) AS MonthYear,
								SaleInvoices.Amount AS Revenue,
								0.0 AS Expenses,
								0.0 AS Profit
							FROM SaleInvoices 
							WHERE 1=1
								AND SaleInvoices.Cancelled = 0
								AND (@FILTER_DateFrom IS NULL OR SaleInvoices.Timestamp >= @FILTER_DateFrom)
								AND (@FILTER_DateTo IS NULL OR SaleInvoices.Timestamp <= @FILTER_DateTo)
						) summarytable
						GROUP BY summarytable.MonthYear
						ORDER BY summarytable.MonthYear ASC
                    ",
                    DBConnection.getSqlParameter("FILTER_DateFrom", FILTER_DateFrom),
                    DBConnection.getSqlParameter("FILTER_DateTo", Util.getAsEndDate(FILTER_DateTo))
                ).ToList();
        }

        /******************************************************************************************************************************************************/
    }
}