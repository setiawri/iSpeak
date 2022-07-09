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
        public List<string> DatasetStackName { get; set; }
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

            DataTable dtRevenues = new DataTable();
            DataTable dtExpenses = new DataTable();
            DataTable dtProfits = new DataTable();
            dtRevenues.Columns.Add(new DataColumn(MONTHCOLUMN, typeof(string)));
            dtExpenses.Columns.Add(new DataColumn(MONTHCOLUMN, typeof(string)));
            dtProfits.Columns.Add(new DataColumn(MONTHCOLUMN, typeof(string)));
            foreach (IncomeStatementModel model in models)
                if (!dtRevenues.Columns.Contains(model.Year))
                {
                    dtRevenues.Columns.Add(new DataColumn(model.Year, typeof(decimal)));
                    dtExpenses.Columns.Add(new DataColumn(model.Year, typeof(decimal)));
                    dtProfits.Columns.Add(new DataColumn(model.Year, typeof(decimal)));
                }

            string[] monthNames = System.Globalization.DateTimeFormatInfo.CurrentInfo.MonthNames;
            DataRow rowRevenues;
            DataRow rowExpenses;
            DataRow rowProfits;
            for (int i = 0; i < 12; i++)
            {
                rowRevenues = dtRevenues.NewRow();
                rowExpenses = dtExpenses.NewRow();
                rowProfits = dtProfits.NewRow();

                //set months in the first column
                rowRevenues[MONTHCOLUMN] = monthNames[i];
                rowExpenses[MONTHCOLUMN] = monthNames[i];
                rowProfits[MONTHCOLUMN] = monthNames[i];

                //set values
                foreach (IncomeStatementModel item in models.Where(x => x.Month == i+1).ToList())
                {
                    rowRevenues[item.Year] = item.Revenues;
                    rowExpenses[item.Year] = item.Expenses;
                    rowProfits[item.Year] = item.Profits;
                }

                //set empty cells to 0
                foreach (DataColumn column in dtRevenues.Columns)
                    if (rowRevenues[column] == DBNull.Value)
                        rowRevenues[column] = 0;
                foreach (DataColumn column in dtExpenses.Columns)
                    if (rowExpenses[column] == DBNull.Value)
                        rowExpenses[column] = 0;
                foreach (DataColumn column in dtProfits.Columns)
                    if (rowProfits[column] == DBNull.Value)
                        rowProfits[column] = 0;

                //add row to tables
                dtRevenues.Rows.Add(rowRevenues);
                dtExpenses.Rows.Add(rowExpenses);
                dtProfits.Rows.Add(rowProfits);
            }

            ChartData chartData = new ChartData();

            //month labels in X axis
            string[] Labels = (dtRevenues.AsEnumerable().Select(p => p.Field<string>(MONTHCOLUMN))).Distinct().ToArray();
            chartData.Labels = Labels;

            //legends
            List<string> datasetLabels = new List<string>();
            for (int i = 1; i < dtExpenses.Columns.Count; i++)
            {
                datasetLabels.Add(dtExpenses.Columns[i].ColumnName + " Expenses");
                datasetLabels.Add(dtRevenues.Columns[i].ColumnName + " Profits");
            }
            chartData.DatasetLabels = datasetLabels.ToArray();

            //data
            List<string> datasetStackName = new List<string>();
            List<decimal[]> datasetDatas = new List<decimal[]>();
            List<decimal> data;
            for (int i = 0; i < dtExpenses.Columns.Count - 1; i++)
            {
                data = new List<decimal>();
                for (int j = 0; j < Labels.Length; j++)
                {
                    decimal amount = (dtExpenses.AsEnumerable().Where(p => p.Field<string>(MONTHCOLUMN) == Labels[j]).Select(p => p.Field<Decimal>(dtExpenses.Columns[i + 1].ColumnName))).FirstOrDefault();
                    data.Add(amount);
                }
                datasetDatas.Add(data.ToArray());
                datasetStackName.Add(datasetLabels[i]);

                data = new List<decimal>();
                for (int j = 0; j < Labels.Length; j++)
                {
                    //use profit amount because chart is stacked with expense. Revenue = profit + expense
                    decimal amount = (dtProfits.AsEnumerable().Where(p => p.Field<string>(MONTHCOLUMN) == Labels[j]).Select(p => p.Field<Decimal>(dtExpenses.Columns[i + 1].ColumnName))).FirstOrDefault();
                    data.Add(amount);
                }
                datasetDatas.Add(data.ToArray());
                datasetStackName.Add(datasetLabels[i]);
            }
            chartData.DatasetDatas = datasetDatas;
            chartData.DatasetStackName = datasetStackName;

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
						SELECT Revenues.*,
							SUBSTRING(Revenues.MonthYear, 0, 5) AS [Year],
							CAST(SUBSTRING(Revenues.MonthYear, 6, 2) AS INT) AS [Month],
							CAST(COALESCE(Revenues.Amount, 0) AS BIGINT) AS Revenues,
							CAST(COALESCE(Expenses.Amount, 0) AS BIGINT) AS Expenses,
							CAST(( (CAST(COALESCE(Revenues.Amount, 0) AS FLOAT) - COALESCE(Expenses.Amount, 0)) ) AS BIGINT) AS Profits,
							CAST(( (CAST(COALESCE(Revenues.Amount, 0) AS FLOAT) - COALESCE(Expenses.Amount, 0)) / IIF(COALESCE(Revenues.Amount, 0) = 0, 1, COALESCE(Revenues.Amount, 0)) * 100) AS DECIMAL(5,2)) AS ProfitPercent
						FROM (SELECT Summary.MonthYear as MonthYear, CAST(SUM(Summary.Amount) AS BIGINT) AS Amount
								FROM (
									SELECT 
										CAST(YEAR(SaleInvoices.Timestamp) AS VARCHAR(4)) + '-' + (SELECT RIGHT('00' + CAST(MONTH(SaleInvoices.Timestamp) AS VARCHAR(2)),2)) AS MonthYear,
										SaleInvoices.Amount AS Amount,
										0.0 AS Profit
									FROM SaleInvoices 
									WHERE 1=1
										AND SaleInvoices.Cancelled = 0
										AND (@FILTER_DateFrom IS NULL OR SaleInvoices.Timestamp >= @FILTER_DateFrom)
										AND (@FILTER_DateTo IS NULL OR SaleInvoices.Timestamp <= @FILTER_DateTo)
								) Summary
								GROUP BY Summary.MonthYear
							) Revenues
							LEFT JOIN (
								SELECT Summary.MonthYear as MonthYear, CAST(SUM(Summary.Amount) AS BIGINT) AS Amount
								FROM (
									SELECT 
										CAST(YEAR(PayrollPaymentItems.Timestamp) AS VARCHAR(4)) + '-' + (SELECT RIGHT('00' + CAST(MONTH(PayrollPaymentItems.Timestamp) AS VARCHAR(2)),2)) AS MonthYear,
										PayrollPaymentItems.Amount AS Amount
									FROM PayrollPaymentItems 
									WHERE 1=1
										AND PayrollPaymentItems.CancelNotes IS NULL
										AND PayrollPaymentItems.Timestamp IS NOT NULL
										AND (@FILTER_DateFrom IS NULL OR PayrollPaymentItems.Timestamp >= @FILTER_DateFrom)
										AND (@FILTER_DateTo IS NULL OR PayrollPaymentItems.Timestamp <= @FILTER_DateTo)
								) Summary
								GROUP BY Summary.MonthYear
							) Expenses ON Expenses.MonthYear = Revenues.MonthYear
						ORDER BY Revenues.MonthYear ASC
                    ",
                    DBConnection.getSqlParameter("FILTER_DateFrom", FILTER_DateFrom),
                    DBConnection.getSqlParameter("FILTER_DateTo", Util.getAsEndDate(FILTER_DateTo))
                ).ToList();
        }

        /******************************************************************************************************************************************************/
    }
}