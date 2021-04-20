using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using Newtonsoft.Json;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class SaleInvoicesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: SaleInvoices
        public ActionResult Index(int? rss, string FILTER_Keyword, string FILTER_PaymentNo, int? FILTER_Cancelled, int? FILTER_Approved, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (!UserAccountsController.getUserAccess(Session).SaleInvoices_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (rss != null && FILTER_DateFrom == null)
            {
                FILTER_chkDateFrom = true;
                FILTER_DateFrom = DateTime.Today.AddMonths(-2);
            }

            setViewBag(FILTER_Keyword, FILTER_PaymentNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            if (rss == null || !string.IsNullOrEmpty(FILTER_Keyword))
                return View(get(FILTER_Keyword, FILTER_PaymentNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo));
            else
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
        }

        // POST: SaleInvoices
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, string FILTER_PaymentNo, int? FILTER_Cancelled, int? FILTER_Approved, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            setViewBag(FILTER_Keyword, FILTER_PaymentNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            return View(get(FILTER_Keyword, FILTER_PaymentNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: SaleInvoices/Create
        public ActionResult Create(string FILTER_Keyword, string FILTER_PaymentNo, int? FILTER_Cancelled, int? FILTER_Approved, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (!UserAccountsController.getUserAccess(Session).SaleInvoices_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_PaymentNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            return View(new SaleInvoicesModel());
        }

        // POST: SaleInvoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SaleInvoicesModel model, string JsonSaleInvoiceItems, string FILTER_Keyword, string FILTER_PaymentNo, int? FILTER_Cancelled, int? FILTER_Approved, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(JsonSaleInvoiceItems))
            {                
                add(model, JsonConvert.DeserializeObject<List<SaleInvoiceItemsModel>>(JsonSaleInvoiceItems));
                return RedirectToAction(nameof(Index), new { 
                    FILTER_Keyword = FILTER_Keyword,
                    FILTER_Cancelled = FILTER_Cancelled,
                    FILTER_Approved = FILTER_Approved,
                    FILTER_chkDateFrom = FILTER_chkDateFrom,
                    FILTER_DateFrom = FILTER_DateFrom,
                    FILTER_chkDateTo = FILTER_chkDateTo,
                    FILTER_DateTo = FILTER_DateTo
                });
            }

            setViewBag(FILTER_Keyword, FILTER_PaymentNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            return View(model);
        }

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, string FILTER_PaymentNo, int? FILTER_Cancelled, int? FILTER_Approved, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Cancelled = FILTER_Cancelled;
            ViewBag.FILTER_Approved = FILTER_Approved;
            ViewBag.FILTER_chkDateFrom = FILTER_chkDateFrom;
            ViewBag.FILTER_DateFrom = FILTER_DateFrom;
            ViewBag.FILTER_chkDateTo = FILTER_chkDateTo;
            ViewBag.FILTER_DateTo = FILTER_DateTo;
            ViewBag.FILTER_PaymentNo = FILTER_PaymentNo;
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

            List<SaleInvoiceItemsModel> models = SaleInvoiceItemsController.get(null, id);
            string content = string.Format(@"
                    <div class='table-responsive'>
                        <table class='table table-striped table-bordered'>
                            <!--<thead>
                                <tr>
                                    <th>Line</th>
                                    <th>Description</th>
                                    <th></th>
                                    <th></th>
                                    <th></th>
                                </tr>
                            </thead>-->
                            <tbody>
                ");

            foreach (SaleInvoiceItemsModel model in models)
            {
                int voucher = model.VouchersAmount;

                string subtractions = "";
                if (voucher != 0)
                    subtractions = Util.append(subtractions, string.Format("Voucher: {0:N0}", -1 * voucher), "<br/>");
                if (model.DiscountAmount != 0)
                    subtractions = Util.append(subtractions, string.Format("Discount: {0:N0}", -1 * model.DiscountAmount), "<br/>");
                if (!string.IsNullOrEmpty(subtractions))
                    subtractions += "<br/>";

                string log = model.RowNo.ToString();
                if (access.SaleInvoices_TutorTravelCost_View)
                    log = string.Format("<a href='javascript: void(0)' onclick=\"Log('{0}')\">{1}</a>", model.Id.ToString(), model.RowNo);

                content += string.Format(@"
                            <tr>
                                <td class='align-top' style='width:10px;'>{0}</td>
                                <td class='align-top'>{1}{2}{3}{4}</td>
                                <td class='align-top text-right'>
                                    Qty: {5:N0} x {6:N0}
                                    {7}{8}
                                </td>
                                <td class='align-top text-right'>
                                    {9}
                                    <strong>{10:N0}</strong>
                                </td>
                            </tr>
                        ",
                        log,
                        model.Description,
                        model.LessonPackages_Id != null ? string.Format("<br/><strong>Available Hours:</strong> {0:N0}", model.SessionHours_Remaining) : "",
                        !string.IsNullOrWhiteSpace(model.VouchersName) ? string.Format("<br/>Vouchers: {0}", model.VouchersName) : "",
                        !string.IsNullOrWhiteSpace(model.Notes) ? string.Format("<br/>Notes: {0}", model.Notes) : "",
                        model.Qty,
                        model.Price,
                        model.TravelCost > 0 || model.TutorTravelCost > 0 ? string.Format("<br/>Travel: {0:N0}", model.TravelCost) : "",
                        access.SaleInvoices_TutorTravelCost_View && (model.TravelCost > 0 || model.TutorTravelCost > 0) ? string.Format(" (Tutor: {0:N0})", model.TutorTravelCost) : "",
                        subtractions,
                        (model.Qty * model.Price) + model.TravelCost - model.DiscountAmount - voucher
                    );
            }

            SaleInvoicesModel SaleInvoice = get(id);
            content += string.Format(@"
                        </tbody></table></div>
                        <div class='mt-2'>
                            <div class='h3 ml-2 float-right font-weight-bold'>TOTAL: {0:N0}</div>
                            {1}
                        </div>
                    ", 
                    SaleInvoice.Amount,
                    !string.IsNullOrWhiteSpace(SaleInvoice.Notes) ? string.Format("<div><strong>Notes: </strong>{0}</div>", SaleInvoice.Notes) : ""
                );

            return Json(new { content = content }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Update_IsChecked(Guid id, bool value)
        {
            update_IsChecked(id, value);
            return Json(new { Message = "" });
        }

        public JsonResult Update_Cancelled(Guid id, string notes)
        {
            update_CancelNotes(id, notes);
            return Json(new { Message = "" });
        }

        //public JsonResult CreatePayment(string JsonIdList)
        //{
        //    if (true)
        //    {
        //        Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
        //        return Json("error nih");
        //    }
        //    else
        //    {
        //        update_CancelNotes(id, notes);
        //    }
        //    return Json(new { Message = "" });
        //}

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public List<SaleInvoicesModel> get(string FILTER_Keyword, string FILTER_PaymentNo, int? FILTER_Cancelled, int? FILTER_Approved,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo) 
        { return get(Session, null, FILTER_Keyword, FILTER_PaymentNo, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo, FILTER_Cancelled, FILTER_Approved); }
        public SaleInvoicesModel get(Guid Id) { return get(Session, Id, null, null, false, null, false, null, null, null).FirstOrDefault(); }
        public static List<SaleInvoicesModel> get(HttpSessionStateBase Session, Guid? Id, string FILTER_Keyword, string FILTER_PaymentNo, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo, 
            int? Cancelled, int? IsChecked)
        {
            Guid Branches_Id = Helper.getActiveBranchId(Session);

            if (FILTER_chkDateFrom == null || !(bool)FILTER_chkDateFrom)
                FILTER_DateFrom = null;

            if (FILTER_chkDateTo == null || !(bool)FILTER_chkDateTo)
                FILTER_DateTo = null;

            return new DBContext().Database.SqlQuery<SaleInvoicesModel>(@"
                    SELECT SaleInvoices.*,
                        Branches.Name AS Branches_Name,
                        Customer_UserAccounts.Fullname AS Customer_UserAccounts_Name
                    FROM SaleInvoices
                        LEFT JOIN Branches ON Branches.Id = SaleInvoices.Branches_Id
                        LEFT JOIN UserAccounts Customer_UserAccounts ON Customer_UserAccounts.Id = SaleInvoices.Customer_UserAccounts_Id
                    WHERE 1=1
						AND (@Id IS NULL OR SaleInvoices.Id = @Id)
						AND (@Id IS NOT NULL OR (
    						(@FILTER_Keyword IS NULL OR (SaleInvoices.No LIKE '%'+@FILTER_Keyword+'%'))
    						AND (@FILTER_PaymentNo IS NULL OR (SaleInvoices.Id IN (                                
                                SELECT SaleInvoices.Id
                                FROM PaymentItems 
	                                LEFT JOIN Payments ON Payments.Id = PaymentItems.Payments_Id
	                                LEFT JOIN SaleInvoices ON Saleinvoices.Id = PaymentItems.ReferenceId
                                WHERE Payments.No = @FILTER_PaymentNo
                            )))
                            AND (@FILTER_DateFrom IS NULL OR SaleInvoices.Timestamp >= @FILTER_DateFrom)
                            AND (@FILTER_DateTo IS NULL OR SaleInvoices.Timestamp <= @FILTER_DateTo)
                            AND (@Cancelled IS NULL OR SaleInvoices.Cancelled = @Cancelled)
                            AND (@IsChecked IS NULL OR SaleInvoices.IsChecked = @IsChecked)
                            AND (@Branches_Id IS NULL OR SaleInvoices.Branches_Id = @Branches_Id)
                        ))
					ORDER BY SaleInvoices.No DESC
                ",
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                DBConnection.getSqlParameter("FILTER_PaymentNo", FILTER_PaymentNo),
                DBConnection.getSqlParameter("FILTER_DateFrom", FILTER_DateFrom),
                DBConnection.getSqlParameter("FILTER_DateTo", Util.getAsEndDate(FILTER_DateTo)),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Branches_Id.Name, Branches_Id),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Cancelled.Name, Cancelled),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_IsChecked.Name, IsChecked)
            ).ToList();
        }

        public void update_IsChecked(Guid Id, bool value)
        {
            db.Database.ExecuteSqlCommand(@"
                UPDATE SaleInvoices 
                SET
                    IsChecked = @IsChecked
                WHERE SaleInvoices.Id = @Id;                
            ",
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_IsChecked.Name, value)
            );

            ActivityLogsController.AddEditLog(db, Session, Id, string.Format(SaleInvoicesModel.COL_IsChecked.LogDisplay, null, value));
            db.SaveChanges();
        }

        public void update_CancelNotes(Guid Id, string CancelNotes)
        {
            db.Database.ExecuteSqlCommand(@"
                UPDATE SaleInvoices 
                SET
                    Cancelled = 1,
                    CancelNotes = @CancelNotes
                WHERE SaleInvoices.Id = @Id;                
            ",
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_CancelNotes.Name, CancelNotes)
            );

            ActivityLogsController.AddEditLog(db, Session, Id, string.Format(SaleInvoicesModel.COL_CancelNotes.LogDisplay, CancelNotes));
            db.SaveChanges();
        }

        public void add(SaleInvoicesModel model, List<SaleInvoiceItemsModel> SaleInvoiceItems)
        {
            model.Branches_Id = Helper.getActiveBranchId(Session);
            model.Timestamp = DateTime.Now;
            model.Due = model.Amount;

            db.Database.ExecuteSqlCommand(@"
                
	            -- INCREMENT LAST HEX NUMBER
	            DECLARE @HexLength int = 5, @LastHex_String varchar(5), @NewNo varchar(5)
	            SELECT @LastHex_String = ISNULL(MAX(No),'') From SaleInvoices	
	            DECLARE @LastHex_Int int
	            SELECT @LastHex_Int = CONVERT(INT, CONVERT(VARBINARY, REPLICATE('0', LEN(@LastHex_String)%2) + @LastHex_String, 2)) --@LastHex_String length must be even number of digits to convert to int
	            SET @NewNo = RIGHT(CONVERT(NVARCHAR(10), CONVERT(VARBINARY(8), @LastHex_Int + 1), 1),@HexLength)

                INSERT INTO SaleInvoices   (Id, No,    Branches_Id, Timestamp, Notes, Customer_UserAccounts_Id, Amount, Due, Cancelled, IsChecked) 
                                    VALUES(@Id,@NewNo,@Branches_Id,@Timestamp,@Notes,@Customer_UserAccounts_Id,@Amount,@Due,@Cancelled,@IsChecked);
            ",
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Branches_Id.Name, model.Branches_Id),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Timestamp.Name, model.Timestamp),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Customer_UserAccounts_Id.Name, model.Customer_UserAccounts_Id),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Amount.Name, model.Amount),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Due.Name, model.Due),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Cancelled.Name, model.Cancelled),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_IsChecked.Name, model.IsChecked)
            );

            ActivityLogsController.AddCreateLog(db, Session, model.Id);
            db.SaveChanges();

            SaleInvoiceItemsController.add(SaleInvoiceItems, model.Id);
        }

        /******************************************************************************************************************************************************/
    }
}