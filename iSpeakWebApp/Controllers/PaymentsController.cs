﻿using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using Newtonsoft.Json;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: Payments
        public ActionResult Index(int? rss, string FILTER_Keyword, string FILTER_InvoiceNo, int? FILTER_Cancelled, int? FILTER_Approved, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (!UserAccountsController.getUserAccess(Session).Payments_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (rss != null && FILTER_DateFrom == null)
            {
                FILTER_chkDateFrom = true;
                FILTER_DateFrom = DateTime.Today.AddMonths(-2);
            }

            setViewBag(FILTER_Keyword, FILTER_InvoiceNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                return View(get(FILTER_Keyword, FILTER_InvoiceNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo));
            }
        }

        // POST: Payments
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, string FILTER_InvoiceNo, int? FILTER_Cancelled, int? FILTER_Approved, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            setViewBag(FILTER_Keyword, FILTER_InvoiceNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            return View(get(FILTER_Keyword, FILTER_InvoiceNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: Payments/Create
        public ActionResult Create(string FILTER_Keyword, string FILTER_InvoiceNo, int? FILTER_Cancelled, int? FILTER_Approved, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (!UserAccountsController.getUserAccess(Session).Payments_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_InvoiceNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            return View(new PaymentsModel());
        }

        // POST: Payments/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(PaymentsModel model, string JsonSaleInvoiceItems, string FILTER_Keyword, string FILTER_InvoiceNo, int? FILTER_Cancelled, int? FILTER_Approved, 
        //    bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        //{
        //    if (ModelState.IsValid && !string.IsNullOrEmpty(JsonSaleInvoiceItems))
        //    {                
        //        add(model, JsonConvert.DeserializeObject<List<SaleInvoiceItemsModel>>(JsonSaleInvoiceItems));
        //        return RedirectToAction(nameof(Index), new { 
        //            FILTER_Keyword = FILTER_Keyword,
        //            FILTER_Cancelled = FILTER_Cancelled,
        //            FILTER_Approved = FILTER_Approved,
        //            FILTER_chkDateFrom = FILTER_chkDateFrom,
        //            FILTER_DateFrom = FILTER_DateFrom,
        //            FILTER_chkDateTo = FILTER_chkDateTo,
        //            FILTER_DateTo = FILTER_DateTo
        //        });
        //    }

        //    setViewBag(FILTER_Keyword, FILTER_InvoiceNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
        //    return View(model);
        //}

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, string FILTER_InvoiceNo, int? FILTER_Cancelled, int? FILTER_Approved, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Cancelled = FILTER_Cancelled;
            ViewBag.FILTER_Approved = FILTER_Approved;
            ViewBag.FILTER_chkDateFrom = FILTER_chkDateFrom;
            ViewBag.FILTER_DateFrom = FILTER_DateFrom;
            ViewBag.FILTER_chkDateTo = FILTER_chkDateTo;
            ViewBag.FILTER_DateTo = FILTER_DateTo;
            ViewBag.FILTER_InvoiceNo = FILTER_InvoiceNo;
            LessonPackagesController.setDropDownListViewBag(this);
            LessonPackagesController.setViewBag(this);
            ProductsController.setDropDownListViewBag(this);
            ServicesController.setDropDownListViewBag(this);
            VouchersController.setDropDownListViewBag(this);
            VouchersController.setViewBag(this);
        }

        public JsonResult GetDetails(Guid id)
        {
            UserAccountRolesModel access = UserAccountsController.getUserAccess(Session);

            List<PaymentItemsModel> models = PaymentItemsController.get(null, id);
            string content = string.Format(@"
                    <div class='table-responsive'>
                        <table class='table table-striped table-bordered'>
                            <thead>
                                <tr>
                                    <th>Invoice</th>
                                    <th class='text-right'>Due before</th>
                                    <th class='text-right'>Payment</th>
                                    <th class='text-right'>Due now</th>
                                </tr>
                            </thead>
                            <tbody>
                ");

            string saleInvoiceLink;
            foreach (PaymentItemsModel model in models)
            {
                saleInvoiceLink = !access.SaleInvoices_View ? model.SaleInvoices_No : 
                    string.Format("<a href='/SaleInvoices?FILTER_chkDateFrom=false&FILTER_chkDateTo=false&FILTER_Keyword={0}' target='_blank'>{0}</a>", model.SaleInvoices_No);

                content += string.Format(@"
                            <tr>
                                <td style='width:50px;'>{0}</td>
                                <td class='text-right'>{1:N0}</td>
                                <td class='text-right'><strong>{2:N0}</strong></td>
                                <td class='text-right'>{3:N0}</td>
                            </tr>
                        ",
                        saleInvoiceLink,
                        model.DueBefore,
                        model.Amount,
                        model.DueAfter
                    );
            }

            PaymentsModel payment = get(id);
            content += string.Format(@"
                        </tbody></table></div>
                        <div class='mt-2'>
                            <div class='h3 ml-2 float-right font-weight-bold'>TOTAL: {0:N0}</div>
                        </div>
                    ",
                    payment.CashAmount + payment.DebitAmount + payment.ConsignmentAmount
                );

            return Json(new { content = content }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Update_Confirmed(Guid id, bool value)
        {
            update_Confirmed(id, value);
            return Json(new { Message = "" });
        }

        public JsonResult Update_Cancelled(Guid id, string notes)
        {
            update_CancelNotes(id, notes);
            return Json(new { Message = "" });
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public List<PaymentsModel> get(string FILTER_Keyword, string FILTER_InvoiceNo, int? FILTER_Cancelled, int? FILTER_Approved,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo) 
        { return get(Session, null, FILTER_Keyword, FILTER_InvoiceNo, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo, FILTER_Cancelled, FILTER_Approved); }
        public PaymentsModel get(Guid Id) { return get(Session, Id, null, null, false, null, false, null, null, null).FirstOrDefault(); }
        public static List<PaymentsModel> get(HttpSessionStateBase Session, Guid? Id, string FILTER_Keyword, string FILTER_InvoiceNo, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo, 
            int? Cancelled, int? IsChecked)
        {
            Guid Branches_Id = Helper.getActiveBranchId(Session);

            if (FILTER_chkDateFrom == null || !(bool)FILTER_chkDateFrom)
                FILTER_DateFrom = null;

            if (FILTER_chkDateTo == null || !(bool)FILTER_chkDateTo)
                FILTER_DateTo = null;

            return new DBContext().Database.SqlQuery<PaymentsModel>(@"
                    SELECT Payments.*
                    FROM Payments
                    WHERE 1=1
						AND (@Id IS NULL OR Payments.Id = @Id)
						AND (@Id IS NOT NULL OR (
    						(@FILTER_Keyword IS NULL OR (Payments.No LIKE '%'+@FILTER_Keyword+'%'))
                            AND (@FILTER_DateFrom IS NULL OR Payments.Timestamp >= @FILTER_DateFrom)
                            AND (@FILTER_DateTo IS NULL OR Payments.Timestamp <= @FILTER_DateTo)
                            AND (@Cancelled IS NULL OR Payments.Cancelled = @Cancelled)
                            AND (@Confirmed IS NULL OR Payments.Confirmed = @Confirmed)
    						AND (@FILTER_InvoiceNo IS NULL OR (Payments.Id IN (                                
                                SELECT PaymentItems.Payments_Id
                                FROM PaymentItems 
	                                LEFT JOIN SaleInvoices ON Saleinvoices.Id = PaymentItems.ReferenceId
                                WHERE SaleInvoices.No = @FILTER_InvoiceNo
                            )))
                        ))
					ORDER BY Payments.No DESC
                ",
                DBConnection.getSqlParameter(PaymentsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                DBConnection.getSqlParameter("FILTER_InvoiceNo", FILTER_InvoiceNo),
                DBConnection.getSqlParameter("FILTER_DateFrom", FILTER_DateFrom),
                DBConnection.getSqlParameter("FILTER_DateTo", Util.getAsEndDate(FILTER_DateTo)),
                DBConnection.getSqlParameter(PaymentsModel.COL_Cancelled.Name, Cancelled),
                DBConnection.getSqlParameter(PaymentsModel.COL_Confirmed.Name, IsChecked)
            ).ToList();
        }

        public void update_Confirmed(Guid Id, bool value)
        {
            db.Database.ExecuteSqlCommand(@"
                UPDATE Payments 
                SET
                    Confirmed = @Confirmed
                WHERE Payments.Id = @Id;                
            ",
                DBConnection.getSqlParameter(PaymentsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(PaymentsModel.COL_Confirmed.Name, value)
            );

            ActivityLogsController.AddEditLog(db, Session, Id, string.Format(PaymentsModel.COL_Confirmed.LogDisplay, null, value));
            db.SaveChanges();
        }

        public void update_CancelNotes(Guid Id, string CancelNotes)
        {
            db.Database.ExecuteSqlCommand(@"
                UPDATE Payments 
                SET
                    Cancelled = 1,
                    CancelNotes = @CancelNotes
                WHERE Payments.Id = @Id;                
            ",
                DBConnection.getSqlParameter(PaymentsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(PaymentsModel.COL_CancelNotes.Name, CancelNotes)
            );

            ActivityLogsController.AddEditLog(db, Session, Id, string.Format(PaymentsModel.COL_CancelNotes.LogDisplay, CancelNotes));
            db.SaveChanges();
        }

        //public void add(PaymentsModel model, List<SaleInvoiceItemsModel> SaleInvoiceItems)
        //{
        //    model.Branches_Id = Helper.getActiveBranchId(Session);
        //    model.Timestamp = DateTime.Now;
        //    model.Due = model.Amount;

        //    db.Database.ExecuteSqlCommand(@"

        //     -- INCREMENT LAST HEX NUMBER
        //     DECLARE @HexLength int = 5, @LastHex_String varchar(5), @NewNo varchar(5)
        //     SELECT @LastHex_String = ISNULL(MAX(No),'') From Payments	
        //     DECLARE @LastHex_Int int
        //     SELECT @LastHex_Int = CONVERT(INT, CONVERT(VARBINARY, REPLICATE('0', LEN(@LastHex_String)%2) + @LastHex_String, 2)) --@LastHex_String length must be even number of digits to convert to int
        //     SET @NewNo = RIGHT(CONVERT(NVARCHAR(10), CONVERT(VARBINARY(8), @LastHex_Int + 1), 1),@HexLength)

        //        INSERT INTO Payments   (Id, No,    Branches_Id, Timestamp, Notes, Customer_UserAccounts_Id, Amount, Due, Cancelled, IsChecked) 
        //                            VALUES(@Id,@NewNo,@Branches_Id,@Timestamp,@Notes,@Customer_UserAccounts_Id,@Amount,@Due,@Cancelled,@IsChecked);
        //    ",
        //        DBConnection.getSqlParameter(PaymentsModel.COL_Id.Name, model.Id),
        //        DBConnection.getSqlParameter(PaymentsModel.COL_Branches_Id.Name, model.Branches_Id),
        //        DBConnection.getSqlParameter(PaymentsModel.COL_Timestamp.Name, model.Timestamp),
        //        DBConnection.getSqlParameter(PaymentsModel.COL_Notes.Name, model.Notes),
        //        DBConnection.getSqlParameter(PaymentsModel.COL_Customer_UserAccounts_Id.Name, model.Customer_UserAccounts_Id),
        //        DBConnection.getSqlParameter(PaymentsModel.COL_Amount.Name, model.Amount),
        //        DBConnection.getSqlParameter(PaymentsModel.COL_Due.Name, model.Due),
        //        DBConnection.getSqlParameter(PaymentsModel.COL_Cancelled.Name, model.Cancelled),
        //        DBConnection.getSqlParameter(PaymentsModel.COL_IsChecked.Name, model.IsChecked)
        //    );

        //    ActivityLogsController.AddCreateLog(db, Session, model.Id);
        //    db.SaveChanges();

        //    SaleInvoiceItemsController.add(SaleInvoiceItems, model.Id);
        //}

        /******************************************************************************************************************************************************/
    }
}