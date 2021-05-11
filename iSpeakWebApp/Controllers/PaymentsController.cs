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

            setViewBag(FILTER_Keyword, FILTER_InvoiceNo, null, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                List<PaymentsModel> models = get(FILTER_Keyword, FILTER_InvoiceNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo)
                    .OrderByDescending(x => x.No).ToList();
                return View(models);
            }
        }

        // POST: Payments
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, string FILTER_InvoiceNo, int? FILTER_Cancelled, int? FILTER_Approved, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            setViewBag(FILTER_Keyword, FILTER_InvoiceNo, null, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            List<PaymentsModel> models = get(FILTER_Keyword, FILTER_InvoiceNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo)
                .OrderByDescending(x => x.No).ToList();
            return View(models);
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: Payments/Create
        public ActionResult Create(string id)
        {
            if (!UserAccountsController.getUserAccess(Session).Payments_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            return setCreateViewBagsAndReturn(id);
        }

        //POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string id, string JsonPayments)
        {
            List<SaleInvoicesModel> saleinvoices = SaleInvoicesController.get(Session, id).OrderBy(x=>x.Timestamp).ToList();

            if (ModelState.IsValid)
            {
                PaymentsModel payment = JsonConvert.DeserializeObject<PaymentsModel>(JsonPayments);
                payment.Id = Guid.NewGuid();
                payment.No = Util.incrementHexNumber(db.Payments.Max(x => x.No));
                payment.Timestamp = DateTime.Now;

                if (payment.DebitAmount == 0)
                {
                    payment.DebitBank = null;
                    payment.DebitNumber = null;
                    payment.DebitOwnerName = null;
                    payment.DebitRefNo = null;
                }
                
                if (payment.ConsignmentAmount == 0)
                    payment.Consignments_Id = null;

                //create payment items and update sale invoice due amount
                int RemainingPaymentAmount = payment.DebitAmount + payment.CashAmount + payment.ConsignmentAmount;
                int paymentItemAmount = 0;
                int dueBefore = 0;
                int dueAfter = 0;
                foreach(SaleInvoicesModel saleinvoice in saleinvoices)
                {
                    dueBefore = saleinvoice.Due;
                    dueAfter = saleinvoice.Due;
                    if (RemainingPaymentAmount == 0)
                        break;
                    else
                    {
                        if (RemainingPaymentAmount >= saleinvoice.Due)
                            paymentItemAmount = saleinvoice.Due;
                        else
                            paymentItemAmount = RemainingPaymentAmount;

                        RemainingPaymentAmount -= paymentItemAmount;
                        dueAfter -= paymentItemAmount;

                        SaleInvoicesController.update_Due(Session, db, saleinvoice.Id, saleinvoice.Due, saleinvoice.Due - paymentItemAmount);
                        saleinvoice.Due -= paymentItemAmount;
                    }

                    PaymentItemsController.add(db, payment.Id, new PaymentItemsModel {
                        Id = Guid.NewGuid(),
                        Payments_Id = payment.Id,
                        ReferenceId = saleinvoice.Id,
                        Amount = paymentItemAmount,
                        DueBefore = dueBefore,
                        DueAfter = dueAfter
                    });
                }

                //create petty cash
                if (payment.CashAmount > 0)
                {
                    PettyCashRecordsController.add(db, new PettyCashRecordsModel
                    {
                        Id = Guid.NewGuid(),
                        Branches_Id = Helper.getActiveBranchId(Session),
                        RefId = payment.Id,
                        No = "",
                        Timestamp = payment.Timestamp,
                        PettyCashRecordsCategories_Id = SettingsController.get().AutoEntryForCashPayments,
                        Notes = "Cash Payment [" + payment.No + "]",
                        Amount = payment.CashAmount,
                        IsChecked = false,
                        UserAccounts_Id_TEMP = (Guid)UserAccountsController.getUserId(Session),
                        ExpenseCategories_Id = null
                    });
                }

                db.Payments.Add(payment);
                db.SaveChanges();

                return RedirectToAction(nameof(Print), new { id = payment.Id });
            }

            return setCreateViewBagsAndReturn(id);
        }

        public ActionResult setCreateViewBagsAndReturn(string saleInvoiceIdList)
        {
            List<SaleInvoiceItemsModel> SaleInvoiceItems = SaleInvoiceItemsController.get(null, null, saleInvoiceIdList, null, null, null, null)
                .OrderBy(x => x.SaleInvoices_No)
                .ThenBy(x => x.RowNo)
                .ToList();
            ViewBag.TotalAmount = SaleInvoiceItems.Sum(x => x.TotalAmount);

            List<SaleInvoicesModel> saleinvoices = SaleInvoicesController.get(Session, saleInvoiceIdList);
            ViewBag.DueAmount = saleinvoices.Sum(x => x.Due);

            ViewBag.id = saleInvoiceIdList;
            //BanksController.setDropDownListViewBag(this);
            ConsignmentsController.setDropDownListViewBag(this);

            return View(SaleInvoiceItems);
        }

        /* PRINT **********************************************************************************************************************************************/

        // GET: Payments/Create
        public ActionResult Print(Guid? id)
        {
            if (id == null || !UserAccountsController.getUserAccess(Session).Payments_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            PaymentsModel model = get((Guid)id);

            ViewBag.InvoiceHeaderText = new BranchesController().get(Helper.getActiveBranchId(Session)).InvoiceHeaderText;
            ViewData["SaleInvoiceItems"] = SaleInvoiceItemsController.get(null, null, null, model.Id, null, null, null)
                .OrderBy(x => x.SaleInvoices_No)
                .ThenBy(x => x.RowNo)
                .ToList(); 
            ViewData["PaymentItems"] = PaymentItemsController.get(null, model.Id);
            ViewBag.TotalAmount = model.CashAmount + model.ConsignmentAmount + model.DebitAmount;

            return View(model);
        }

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, string FILTER_InvoiceNo, string FILTER_PaymentNo, int? FILTER_Cancelled, int? FILTER_Approved, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_InvoiceNo = FILTER_InvoiceNo;
            ViewBag.FILTER_PaymentNo = FILTER_PaymentNo;
            ViewBag.FILTER_Cancelled = FILTER_Cancelled;
            ViewBag.FILTER_Approved = FILTER_Approved;
            ViewBag.FILTER_chkDateFrom = FILTER_chkDateFrom;
            ViewBag.FILTER_DateFrom = FILTER_DateFrom;
            ViewBag.FILTER_chkDateTo = FILTER_chkDateTo;
            ViewBag.FILTER_DateTo = FILTER_DateTo;
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

        public JsonResult Update_Approval(Guid id, bool value)
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
                    SELECT Payments.*,
                        Consignments.Name AS Consignments_Name
                    FROM Payments
                        LEFT JOIN Consignments ON Consignments.Id = Payments.Consignments_Id
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

        /******************************************************************************************************************************************************/
    }
}