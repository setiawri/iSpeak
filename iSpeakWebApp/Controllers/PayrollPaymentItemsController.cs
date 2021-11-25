using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;
using LIBWebMVC;

namespace iSpeakWebApp.Controllers
{
    public class PayrollPaymentItemsController : Controller
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
                models = getSummary(Util.getAsStartDate(FILTER_DatePeriod).Value, Util.getLastDayOfSelectedMonth(FILTER_DatePeriod.Value).Value);
            }

            return View(models);
        }

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(DateTime? FILTER_DatePeriod)
        {
            ViewBag.FILTER_DatePeriod = FILTER_DatePeriod ?? Util.getFirstDayOfSelectedMonth(Helper.getCurrentDateTime());
        }

        public JsonResult GetDetails(Guid id, DateTime DatePeriod)
        {
            string content = "";

            List<PayrollPaymentItemsModel> models = get(Session, id, DatePeriod, null);

            if(models.Count > 0)
            {
                Guid Tutor_UserAccounts_Id = models[0].UserAccounts_Id;
                string Tutor_UserAccounts_Fullname = models[0].UserAccounts_Fullname;

                List<PayrollPaymentItemsModel> combinedModels = combineClassSesions(models);
                    
                content = string.Format(@"
                        <div class='table-responsive'>
                            <table class='table table-sm table-striped table-bordered'>
                                <thead>
                                    <tr>
                                        <th style='width:140px;'>Date</th>
                                        <th>Description</th>
                                        <th class='text-center' style='width:140px;'>Hours x Payrate</th>
                                        <th class='text-right' style='width:40px;'>Travel</th>
                                        <th class='text-right' style='width:40px;'>Amount</th>
                                        <th class='text-center' style='width:20px;'>Due</th>
                                    </tr>
                                </thead>
                                <tbody>
                    ");

                decimal dueAmount = 0;
                Guid? PayrollPayments_Id = null;
                foreach (PayrollPaymentItemsModel model in combinedModels)
                {
                    dueAmount = model.Amount - model.PayrollPaymentAmount;
                    content += string.Format(@"
                                <tr>
                                    <td class='align-top'>{0:dd/MM/yy HH:mm}</td>
                                    <td class='align-top'>{1}</td>
                                    <td class='align-top text-right'>{2:N2} x {3:N2}</td>
                                    <td class='align-top text-right'>{4:N2}</td>
                                    <td class='align-top text-right'>{5:N2}</td>
                                    <td class='align-top text-center'>{6}</td>
                                </tr>
                            ",
                            model.Timestamp,
                            model.Description,
                            model.Hour,
                            model.HourlyRate,
                            model.TutorTravelCost,
                            model.Amount,
                            dueAmount > 0 ? 
                                string.Format("<a href=\"javascript: void(0)\" onclick=\"Log('{0}')\" class='text-primary'>{1:N0}</a>", model.Id, dueAmount) 
                                : string.Format("<a href=\"javascript: void(0)\" onclick=\"Log('{0}')\" class='text-primary'><i class='icon-checkmark'></i></a>", model.Id)
                        );
                    PayrollPayments_Id = model.PayrollPayments_Id;
                }

                dueAmount = combinedModels.Sum(x => x.Amount - x.PayrollPaymentAmount);
                content += string.Format(@"
                            </tbody></table></div>

                            <div class='mt-2 float-right'>
                                <div class='h3 ml-2'>TOTAL: {0:N2}</div>
                            </div>
                            <div class='mt-2 row'>
                                <div class='h3 ml-2 font-weight-bold'>DUE: {1:N2}</div>
                                <div><button type='button' class='btn btn-success mx-2' data-toggle='modal' data-target='#modal_payment' {2}"
                        + "onclick=\"ClosePayrollItemsDialog('{3}','{4}',{1})\""
                        + @"><i class='icon-checkmark3 mr-2'></i>CREATE PAYMENT</button></div>
                                <div><button type='button' class='btn btn-primary mr-2 {5}' onclick=""Log('{6}')"">PAYMENT INFO</button></div>
                            </ div>
                        ",
                        combinedModels.Sum(x=>x.Amount),
                        dueAmount,
                        dueAmount > 0 ? "" : "disabled='true'",
                        Tutor_UserAccounts_Id,
                        Tutor_UserAccounts_Fullname,
                        PayrollPayments_Id == null ? "d-none" : "",
                        PayrollPayments_Id
                    );
            }

            return Json(new { content = content }, JsonRequestBehavior.AllowGet);
        }

        public static List<PayrollPaymentItemsModel> combineClassSesions(List<PayrollPaymentItemsModel> models)
        {
            List<PayrollPaymentItemsModel> combinedModels = new List<PayrollPaymentItemsModel>();

            PayrollPaymentItemsModel existingModel;
            foreach (PayrollPaymentItemsModel model in models)
            {
                existingModel = combinedModels.Find(x => x.Id == model.Id);
                if (existingModel != null)
                    existingModel.Description = Util.append(existingModel.Description, model.Student_UserAccounts_FirstName, ", ");
                else
                {
                    if (!string.IsNullOrEmpty(model.Student_UserAccounts_FirstName))
                        model.Description = Util.append(model.Description, model.Student_UserAccounts_FirstName, ",");

                    combinedModels.Add(model);
                }
            }

            return combinedModels;
        }

        public JsonResult GenerateFullTimePayroll(DateTime param1)
        {
            int newItems = 0;
            DateTime DatePeriod = param1;

            List<HourlyRatesModel> ActiveFullTimeEmployeePayrates = HourlyRatesController.getActiveFullTimeEmployeePayrates(Session);
            List<PayrollPaymentItemsModel> FullTimePayrollPaymentItems = get(Session, null, DatePeriod, 1);
            DateTime Timestamp = Util.getLastDayOfSelectedMonth(DatePeriod).Value;
            string Description = string.Format("Payroll {0:MMM yyyy}", Timestamp);

            if(ActiveFullTimeEmployeePayrates.Count > 0)
            {
                foreach(HourlyRatesModel model in ActiveFullTimeEmployeePayrates)
                {
                    if (!FullTimePayrollPaymentItems.Exists(x => x.UserAccounts_Id == model.UserAccounts_Id))
                    {
                        add(new PayrollPaymentItemsModel()
                        {
                            Id = Guid.NewGuid(),
                            PayrollPayments_Id = null,
                            Timestamp = Timestamp,
                            Description = Description,
                            Hour = 0,
                            HourlyRate = 0,
                            TutorTravelCost = 0,
                            Amount = model.FullTimeTutorPayrate,
                            UserAccounts_Id = model.UserAccounts_Id,
                            CancelNotes = string.Empty,
                            Branches_Id = model.Branches_Id,
                            IsFullTime = true
                        });
                        newItems++;
                    }
                }
            }

            return Json(new { Message = "Generated " + newItems + " payrolls" });
        }

        public JsonResult Create(Guid? UserAccounts_Id, string Description, DateTime Timestamp, int Amount)
        {
            if(UserAccounts_Id == null)
                UtilWebMVC.Json(Response, "Please select employee");
            else if (string.IsNullOrEmpty(Description))
                UtilWebMVC.Json(Response, "Please provide description");
            else
            {
                Guid Id = Guid.NewGuid();
                add(new PayrollPaymentItemsModel()
                {
                    Id = Id,
                    PayrollPayments_Id = null,
                    Timestamp = Timestamp,
                    Description = Description,
                    Hour = 0,
                    HourlyRate = 0,
                    TutorTravelCost = 0,
                    Amount = Amount,
                    UserAccounts_Id = UserAccounts_Id.Value,
                    CancelNotes = string.Empty,
                    Branches_Id = Helper.getActiveBranchId(Session),
                    IsFullTime = false
                });

                ActivityLogsController.AddCreateLog(db, Session, Id);
                db.SaveChanges();
            }

            return Json(new { Message = "" });
        }

        public static decimal calculateAmount(bool IsWaiveTutorFee, decimal SessionHours, decimal HourlyRate, int TutorTravelCost)
        {
            return IsWaiveTutorFee ? 0 : (SessionHours * HourlyRate) + TutorTravelCost;
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public static PayrollPaymentItemsModel get(HttpSessionStateBase Session, Guid? Id) { return get(Session, Id, null, null, null, null).FirstOrDefault(); }
        public static List<PayrollPaymentItemsModel> get(HttpSessionStateBase Session, Guid? UserAccounts_Id, DateTime? DatePeriod, int? IsFullTime) { return get(Session, null, null, UserAccounts_Id, DatePeriod, IsFullTime); }
        public static List<PayrollPaymentItemsModel> get(HttpSessionStateBase Session, Guid? Id, Guid? PayrollPayments_Id, Guid? UserAccounts_Id, DateTime? DatePeriod, int? IsFullTime)
        {
            return new DBContext().Database.SqlQuery<PayrollPaymentItemsModel>(@"
                    SELECT PayrollPaymentItems.*,
                        UserAccounts.Fullname AS UserAccounts_Fullname,
                        Student_UserAccounts.Fullname AS Student_UserAccounts_Fullname,
                        CASE CHARINDEX(' ', Student_UserAccounts.Fullname, 1)
                             WHEN 0 THEN Student_UserAccounts.Fullname
                             ELSE SUBSTRING(Student_UserAccounts.Fullname, 1, CHARINDEX(' ', Student_UserAccounts.Fullname, 1) - 1)
                        END AS Student_UserAccounts_FirstName,
                        ROW_NUMBER() OVER (ORDER BY PayrollPaymentItems.Timestamp ASC, UserAccounts.Fullname ASC) AS InitialRowNumber
                    FROM PayrollPaymentItems
                        LEFT JOIN LessonSessions ON LessonSessions.PayrollPaymentItems_Id = PayrollPaymentItems.Id
                        LEFT JOIN SaleInvoiceItems ON SaleInvoiceItems.Id = LessonSessions.SaleInvoiceItems_Id
                        LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                        LEFT JOIN UserAccounts ON UserAccounts.Id = PayrollPaymentItems.UserAccounts_Id
						LEFT JOIN UserAccounts Student_UserAccounts ON Student_UserAccounts.Id = SaleInvoices.Customer_UserAccounts_Id
                    WHERE 1=1
						AND (@Id IS NULL OR PayrollPaymentItems.Id = @Id)
						AND (@Id IS NOT NULL OR (
                            PayrollPaymentItems.Branches_Id = @Branches_Id
                            AND (@PayrollPayments_Id IS NULL OR PayrollPaymentItems.PayrollPayments_Id = @PayrollPayments_Id)
                            AND (@UserAccounts_Id IS NULL OR PayrollPaymentItems.UserAccounts_Id = @UserAccounts_Id)
                            AND (@DatePeriod IS NULL OR (MONTH(PayrollPaymentItems.Timestamp) = MONTH(@DatePeriod) AND YEAR(PayrollPaymentItems.Timestamp) = YEAR(@DatePeriod)))
                            AND (@IsFullTime IS NULL OR PayrollPaymentItems.IsFullTime = @IsFullTime)
                        ))
					ORDER BY PayrollPaymentItems.Timestamp ASC, UserAccounts.Fullname ASC
                ",
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Branches_Id.Name, Helper.getActiveBranchId(Session)),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_PayrollPayments_Id.Name, PayrollPayments_Id),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_UserAccounts_Id.Name, UserAccounts_Id),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_IsFullTime.Name, IsFullTime),
                DBConnection.getSqlParameter("DatePeriod", DatePeriod)
            ).ToList();
        }

        public static void add(PayrollPaymentItemsModel model)
        {
            new DBContext().Database.ExecuteSqlCommand(@"
                    INSERT INTO PayrollPaymentItems (Id, PayrollPayments_Id, Timestamp, Description, Hour, HourlyRate, TutorTravelCost, Amount, UserAccounts_Id, CancelNotes, Branches_Id, IsFullTime) 
                                             VALUES(@Id,@PayrollPayments_Id,@Timestamp,@Description,@Hour,@HourlyRate,@TutorTravelCost,@Amount,@UserAccounts_Id,@CancelNotes,@Branches_Id,@IsFullTime);
                ",
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_PayrollPayments_Id.Name, model.PayrollPayments_Id),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Timestamp.Name, model.Timestamp),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Description.Name, model.Description),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Hour.Name, model.Hour),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_HourlyRate.Name, model.HourlyRate),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_TutorTravelCost.Name, model.TutorTravelCost),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Amount.Name, model.Amount),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_UserAccounts_Id.Name, model.UserAccounts_Id),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_CancelNotes.Name, model.CancelNotes),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Branches_Id.Name, model.Branches_Id),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_IsFullTime.Name, model.IsFullTime)
            );
        }

        public static void update_PayrollPayments_Id(DBContext db, HttpSessionStateBase Session, Guid? PayrollPayments_Id, List<PayrollPaymentItemsModel> models)
        {
            foreach (PayrollPaymentItemsModel model in models)
            {
                db.Database.ExecuteSqlCommand(@"
                        UPDATE PayrollPaymentItems 
                        SET PayrollPayments_Id = @PayrollPayments_Id, PayrollPaymentAmount = Amount
                        WHERE Id = @Id;
                    ",
                    DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Id.Name, model.Id),
                    DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_PayrollPayments_Id.Name, PayrollPayments_Id)
                );
            }
        }

        public static void update(DBContext db, HttpSessionStateBase Session, PayrollPaymentItemsModel model)
        {
            PayrollPaymentItemsModel originalModel = get(Session, model.Id);

            string log = string.Empty;
            log = Helper.append(log, originalModel.HourlyRate, model.HourlyRate, PayrollPaymentItemsModel.COL_HourlyRate.LogDisplay);
            log = Helper.append(log, originalModel.TutorTravelCost, model.TutorTravelCost, PayrollPaymentItemsModel.COL_TutorTravelCost.LogDisplay);
            log = Helper.append(log, originalModel.Amount, model.Amount, PayrollPaymentItemsModel.COL_Amount.LogDisplay);

            if (!string.IsNullOrEmpty(log))
            {
                WebDBConnection.Update(db.Database, "PayrollPaymentItems",
                    DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Id.Name, model.Id),
                    DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_HourlyRate.Name, model.HourlyRate),
                    DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_TutorTravelCost.Name, model.TutorTravelCost),
                    DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Amount.Name, model.Amount)
                );
                ActivityLogsController.AddEditLog(db, Session, model.Id, log);
                db.SaveChanges();
            }
        }

        private List<PayrollsModel> getSummary(DateTime StartDate, DateTime EndDate)
        {
            List<PayrollsModel> models = db.Database.SqlQuery<PayrollsModel>(@"
                        SELECT Payrolls.*,
                            '' AS Tutor_UserAccounts_Id,
							ISNULL(Due.Amount,0) AS DueAmount,
							UserAccounts.Fullname AS Tutor_UserAccounts_FullName
						FROM (
								SELECT UserAccounts_Id AS Tutor_UserAccounts_Id,
									SUM(Hour) AS TotalHours,
									SUM(Amount) AS PayableAmount
								FROM PayrollPaymentItems
								WHERE PayrollPaymentItems.Timestamp >= @StartDate AND PayrollPaymentItems.Timestamp <= @EndDate
                                    AND PayrollPaymentItems.Branches_Id = @Branches_Id
								GROUP BY UserAccounts_Id					
							) Payrolls
							LEFT JOIN UserAccounts ON UserAccounts.Id = Payrolls.Tutor_UserAccounts_Id
							LEFT JOIN (
									SELECT UserAccounts_Id AS Tutor_UserAccounts_Id,
										SUM(PayrollPaymentItems.Amount-PayrollPaymentItems.PayrollPaymentAmount) AS Amount
									FROM PayrollPaymentItems
									WHERE PayrollPaymentItems.Timestamp >= @StartDate AND PayrollPaymentItems.Timestamp <= @EndDate
                                        AND PayrollPaymentItems.Branches_Id = @Branches_Id
										AND PayrollPaymentItems.Amount <> PayrollPaymentItems.PayrollPaymentAmount
									GROUP BY UserAccounts_Id						
								) Due ON Due.Tutor_UserAccounts_Id = Payrolls.Tutor_UserAccounts_Id
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