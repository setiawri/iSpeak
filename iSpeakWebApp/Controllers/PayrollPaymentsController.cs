﻿using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;
using LIBWebMVC;

namespace iSpeakWebApp.Controllers
{
    /*
     * Payroll is filtered by Franchise. 
     */

    public class PayrollPaymentsController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: PayrollPayments
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Cancelled, int? FILTER_Approved,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (!UserAccountsController.getUserAccess(Session).PayrollPayments_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                return View(get(FILTER_Keyword, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo));
            }
        }

        // POST: PayrollPayments
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Cancelled, int? FILTER_Approved,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            setViewBag(FILTER_Keyword, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            return View(get(FILTER_Keyword, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo));
        }

        /* PRINT **********************************************************************************************************************************************/

        // GET: Payments/Print
        public ActionResult Print(Guid? id)
        {
            if (id == null || !UserAccountsController.getUserAccess(Session).PayrollPayments_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            PayrollPaymentsModel model = get(Session, (Guid)id);

            ViewBag.InvoiceHeaderText = new BranchesController().get(Helper.getActiveBranchId(Session)).InvoiceHeaderText;
            ViewData["PayrollPaymentItems"] = PayrollPaymentItemsController.get(Session, null, model.Id, null, null, null);
            ViewBag.TotalAmount = model.Amount;

            return View(model);
        }

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Cancelled, int? FILTER_Approved,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Cancelled = FILTER_Cancelled;
            ViewBag.FILTER_Approved = FILTER_Approved;
            ViewBag.FILTER_chkDateFrom = FILTER_chkDateFrom;
            ViewBag.FILTER_DateFrom = FILTER_DateFrom;
            ViewBag.FILTER_chkDateTo = FILTER_chkDateTo;
            ViewBag.FILTER_DateTo = FILTER_DateTo;
        }

        public JsonResult Ajax_Update_Approved(Guid id, bool value)
        {
            update_Approved(id, value);
            return Json(new { Message = "" });
        }

        public JsonResult Ajax_Update_Cancelled(Guid id, string notes)
        {
            update_CancelNotes(id, notes);
            return Json(new { Message = "" });
        }

        public JsonResult Ajax_Create(Guid UserAccounts_Id, string Notes, DateTime Timestamp, decimal Amount, DateTime DatePeriod)
        {
            List<PayrollPaymentItemsModel> PayrollPaymentItems = PayrollPaymentItemsController.combineClassSesions(PayrollPaymentItemsController.get(Session, UserAccounts_Id, DatePeriod, null));

            if (Amount != PayrollPaymentItems.Sum(x => x.Amount - x.PayrollPaymentAmount))
                return UtilWebMVC.Json(Response, "Due amount has changed. Please reload list and try again.");

            add(PayrollPaymentItems, new PayrollPaymentsModel
            {
                Id = Guid.NewGuid(),
                Timestamp = Timestamp,
                Amount = Amount,
                Notes = Notes,
                UserAccounts_Id = UserAccounts_Id, 
                Branches_Id = (Guid)PayrollPaymentItems[0].Branches_Id
            });

            return Json(new { Message = "" });
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public List<PayrollPaymentsModel> get(string FILTER_Keyword, int? FILTER_Cancelled, int? FILTER_Approved, bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo) { return get(Session, null, FILTER_Keyword, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo); }
        public static PayrollPaymentsModel get(HttpSessionStateBase Session, Guid Id) { return get(Session, Id, null, null, null, null, null, null, null).FirstOrDefault(); }
        public static List<PayrollPaymentsModel> get(HttpSessionStateBase Session, Guid? Id, string FILTER_Keyword, int? Cancelled, int? Approved,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (FILTER_chkDateFrom == null || !(bool)FILTER_chkDateFrom)
                FILTER_DateFrom = null;

            if (FILTER_chkDateTo == null || !(bool)FILTER_chkDateTo)
                FILTER_DateTo = null;

            return new DBContext().Database.SqlQuery<PayrollPaymentsModel>(@"
                        SELECT PayrollPayments.*,
                            UserAccounts.Fullname AS UserAccounts_Fullname,
                            Branches.Name AS Branches_Name
                        FROM PayrollPayments
                            LEFT JOIN UserAccounts ON UserAccounts.Id = PayrollPayments.UserAccounts_Id
                            LEFT JOIN Branches ON Branches.Id = PayrollPayments.Branches_Id
                        WHERE 1=1
							AND (@Id IS NULL OR PayrollPayments.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                PayrollPayments.Branches_Id = @Branches_Id
    							AND (@FILTER_Keyword IS NULL OR (PayrollPayments.No LIKE '%'+@FILTER_Keyword+'%' OR UserAccounts.Fullname LIKE '%'+@FILTER_Keyword+'%'))
                                AND (@FILTER_DateFrom IS NULL OR PayrollPayments.Timestamp >= @FILTER_DateFrom)
                                AND (@FILTER_DateTo IS NULL OR PayrollPayments.Timestamp <= @FILTER_DateTo)
                                AND (@Cancelled IS NULL OR PayrollPayments.Cancelled = @Cancelled)
                                AND (@Approved IS NULL OR PayrollPayments.Approved = @Approved)
                                AND (Branches.Franchises_Id = @Franchises_Id)
                            ))
						ORDER BY UserAccounts.Fullname ASC
                    ",
                    DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                    DBConnection.getSqlParameter("FILTER_DateFrom", FILTER_DateFrom),
                    DBConnection.getSqlParameter("FILTER_DateTo", Util.getAsEndDate(FILTER_DateTo)),
                    DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Branches_Id.Name, Helper.getActiveBranchId(Session)),
                    DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Cancelled.Name, Cancelled),
                    DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Approved.Name, Approved),
                    DBConnection.getSqlParameter("Franchises_Id", Helper.getActiveFranchiseId(Session))
                ).ToList();
        }

        public void add(List<PayrollPaymentItemsModel> items, PayrollPaymentsModel model)
        {
            if (items.Count == 0)
                return;

            Guid? PayrollPayments_Id = null;
            foreach(PayrollPaymentItemsModel item in items)
            {
                if(item.PayrollPayments_Id != null)
                {
                    PayrollPayments_Id = item.PayrollPayments_Id;
                    break;
                }
            }

            string log = string.Format("Payment of {0:N0} on {1:dd/MM/yy}", model.Amount, model.Timestamp);
            if (!string.IsNullOrWhiteSpace(model.Notes))
                log += ", Notes: " + model.Notes;

            if (PayrollPayments_Id != null)
            {
                log = "Additional " + log;
                model = get(Session, (Guid)PayrollPayments_Id);
            }
            else
            {
                model.Id = Guid.NewGuid();

                db.Database.ExecuteSqlCommand(@"
	                -- INCREMENT LAST HEX NUMBER
	                DECLARE @HexLength int = 5, @LastHex_String varchar(5), @NewNo varchar(5)
	                SELECT @LastHex_String = ISNULL(MAX(No),'') From PayrollPayments	
	                DECLARE @LastHex_Int int
	                SELECT @LastHex_Int = CONVERT(INT, CONVERT(VARBINARY, REPLICATE('0', LEN(@LastHex_String)%2) + @LastHex_String, 2)) --@LastHex_String length must be even number of digits to convert to int
	                SET @NewNo = RIGHT(CONVERT(NVARCHAR(10), CONVERT(VARBINARY(8), @LastHex_Int + 1), 1),@HexLength)

                INSERT INTO PayrollPayments (Id, No,    Timestamp, UserAccounts_Id, Amount, Branches_Id, Approved, Cancelled, CancelNotes, Notes) 
                                     VALUES(@Id,@NewNo,@Timestamp,@UserAccounts_Id,@Amount,@Branches_Id,@Approved,@Cancelled,@CancelNotes,@Notes);
            ",
                    DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Id.Name, model.Id),
                    DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Timestamp.Name, model.Timestamp),
                    DBConnection.getSqlParameter(PayrollPaymentsModel.COL_No.Name, model.No),
                    DBConnection.getSqlParameter(PayrollPaymentsModel.COL_UserAccounts_Id.Name, model.UserAccounts_Id),
                    DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Amount.Name, model.Amount),
                    DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Branches_Id.Name, model.Branches_Id),
                    DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Approved.Name, model.Approved),
                    DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Cancelled.Name, model.Cancelled),
                    DBConnection.getSqlParameter(PayrollPaymentsModel.COL_CancelNotes.Name, model.CancelNotes),
                    DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Notes.Name, model.Notes)
                );
            }
            ActivityLogsController.Add(db, Session, model.Id, log);

            PayrollPaymentItemsController.update_PayrollPayments_Id(db, Session, model.Id, items);
        }

        public void update_Approved(Guid Id, bool value)
        {
            WebDBConnection.Update(db.Database, "PayrollPayments",
                DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Approved.Name, value)
            );

            ActivityLogsController.AddEditLog(db, Session, Id, string.Format(PayrollPaymentsModel.COL_Approved.LogDisplay, null, value));
        }

        public void update_CancelNotes(Guid Id, string CancelNotes)
        {
            db.Database.ExecuteSqlCommand(@"
                UPDATE PayrollPaymentItems SET PayrollPayments_Id = NULL WHERE PayrollPayments_Id = @Id;

                UPDATE PayrollPayments 
                SET
                    Cancelled = 1,
                    CancelNotes = @CancelNotes
                WHERE PayrollPayments.Id = @Id;                
            ",
                DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(PayrollPaymentsModel.COL_CancelNotes.Name, CancelNotes)
            );

            ActivityLogsController.AddEditLog(db, Session, Id, string.Format(PayrollPaymentsModel.COL_CancelNotes.LogDisplay, CancelNotes));
        }

        /******************************************************************************************************************************************************/
    }
}