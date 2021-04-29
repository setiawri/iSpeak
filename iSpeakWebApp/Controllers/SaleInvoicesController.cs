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

            setViewBag(FILTER_Keyword, FILTER_PaymentNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            if (rss == null || !string.IsNullOrEmpty(FILTER_Keyword))
            {
                List<SaleInvoicesModel> models = get(FILTER_Keyword, FILTER_PaymentNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo)
                    .OrderByDescending(x => x.No).ToList();
                return View(models);
            }
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
            List<SaleInvoicesModel> models = get(FILTER_Keyword, FILTER_PaymentNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo)
                .OrderByDescending(x => x.No).ToList();
            return View(models);
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
        public ActionResult Create(SaleInvoicesModel model, string JsonSaleInvoiceItems, 
            string FILTER_Keyword, string FILTER_PaymentNo, int? FILTER_Cancelled, int? FILTER_Approved, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            List<SaleInvoiceItemsModel> SaleInvoiceItems = new List<SaleInvoiceItemsModel>();
            if(!string.IsNullOrEmpty(JsonSaleInvoiceItems))
                SaleInvoiceItems = JsonConvert.DeserializeObject<List<SaleInvoiceItemsModel>>(JsonSaleInvoiceItems);

            string errorMessage = "";
            if (ModelState.IsValid)
            {
                if(SaleInvoiceItems.Count == 0)
                    UtilWebMVC.setBootboxMessage(this, "Please add at least 1 item");
                else if(!hasSufficientInventory(SaleInvoiceItems, out errorMessage))
                    UtilWebMVC.setBootboxMessage(this, errorMessage); 
                else
                {
                    add(model, SaleInvoiceItems);
                    return RedirectToAction(nameof(Index), new { rss = 1 });

                    //not working. all filter returns null
                    //return RedirectToAction(nameof(Index), new
                    //{
                    //    FILTER_Keyword = FILTER_Keyword,
                    //    FILTER_PaymentNo = FILTER_PaymentNo,
                    //    FILTER_Cancelled = FILTER_Cancelled,
                    //    FILTER_Approved = FILTER_Approved,
                    //    FILTER_chkDateFrom = FILTER_chkDateFrom,
                    //    FILTER_DateFrom = FILTER_DateFrom,
                    //    FILTER_chkDateTo = FILTER_chkDateTo,
                    //    FILTER_DateTo = FILTER_DateTo
                    //});
                }
            }

            setViewBag(FILTER_Keyword, FILTER_PaymentNo, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            ViewData["SaleInvoiceItems"] = SaleInvoiceItems;
            return View(model);
        }

        public bool hasSufficientInventory(List<SaleInvoiceItemsModel> SaleInvoiceItems, out string errorMessage)
        {
            errorMessage = "";
            List<ProductsModel> products = ProductsController.get(1, 1);
            foreach(SaleInvoiceItemsModel invoice in SaleInvoiceItems)
            {
                if (invoice.Products_Id != null && SaleInvoiceItems.Where(x => x.Products_Id == invoice.Products_Id).Sum(x => x.Qty) > products.Find(x => x.Id == invoice.Products_Id).AvailableQty)
                {
                    errorMessage = string.Format("Insufficient available quantity in inventory for {0}", products.Find(x => x.Id == invoice.Products_Id).Name);
                    return false;
                }
            }

            return true;
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
            ProductsController.setDropDownListViewBag(this, ProductsModel.COL_DDLDescription.Name, 1, 1);
            ProductsController.setViewBag(this);
            ServicesController.setDropDownListViewBag(this);
            ServicesController.setViewBag(this);
            VouchersController.setDropDownListViewBag(this);
            VouchersController.setViewBag(this);
        }

        public JsonResult GetDetails(Guid id)
        {
            UserAccountRolesModel access = UserAccountsController.getUserAccess(Session);

            List<SaleInvoiceItemsModel> models = SaleInvoiceItemsController.get(null, id, null, null);
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
                        model.TotalAmount
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
            return UtilWebMVC.Json(Response, update_CancelNotes(id, notes));
        }
        
        /* DATABASE METHODS ***********************************************************************************************************************************/

        public List<SaleInvoicesModel> get(string FILTER_Keyword, string FILTER_PaymentNo, int? FILTER_Cancelled, int? FILTER_Approved,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo) 
        { return get(Session, null, null, FILTER_Keyword, FILTER_PaymentNo, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo, FILTER_Cancelled, FILTER_Approved); }
        public static List<SaleInvoicesModel> get(HttpSessionStateBase Session, string SaleInvoiceItemIdList) { return get(Session, null, SaleInvoiceItemIdList, null, null, false, null, false, null, null, null); }
        public SaleInvoicesModel get(Guid Id) { return get(Session, Id, null, null, null, false, null, false, null, null, null).FirstOrDefault(); }
        public static List<SaleInvoicesModel> get(HttpSessionStateBase Session, Guid? Id, string SaleInvoiceItemIdList,
            string FILTER_Keyword, string FILTER_PaymentNo, bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo, 
            int? Cancelled, int? IsChecked)
        {
            Guid Branches_Id = Helper.getActiveBranchId(Session);

            if (FILTER_chkDateFrom == null || !(bool)FILTER_chkDateFrom)
                FILTER_DateFrom = null;

            if (FILTER_chkDateTo == null || !(bool)FILTER_chkDateTo)
                FILTER_DateTo = null;

            string SaleInvoiceItemIdListClause = "";
            if (!string.IsNullOrEmpty(SaleInvoiceItemIdList))
                SaleInvoiceItemIdListClause = string.Format("AND SaleInvoices.Id IN ({0})", UtilWebMVC.convertToSqlIdList(SaleInvoiceItemIdList));

            string sql = string.Format(@"
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
                            {0}
                        ))
					ORDER BY SaleInvoices.No DESC
                ", SaleInvoiceItemIdListClause);

            return new DBContext().Database.SqlQuery<SaleInvoicesModel>(sql,
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

        public static void update_Due(HttpSessionStateBase Session, DBContext db, Guid Id, int originalValue, int newValue)
        {
            db.Database.ExecuteSqlCommand(@"
                UPDATE SaleInvoices 
                SET
                    Due = @Due
                WHERE SaleInvoices.Id = @Id;                
            ",
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Due.Name, newValue)
            );

            ActivityLogsController.AddEditLog(db, Session, Id, string.Format(SaleInvoicesModel.COL_Due.LogDisplay, originalValue, newValue));
            db.SaveChanges();
        }

        public string update_CancelNotes(Guid Id, string CancelNotes)
        {
            SqlQueryResult result = DBConnection.executeQuery("DBContext", @"
                    IF EXISTS(
                            SELECT Payments.Id
                            FROM PaymentItems 
	                            LEFT JOIN Payments ON Payments.Id = PaymentItems.Payments_Id
	                            LEFT JOIN SaleInvoices ON Saleinvoices.Id = PaymentItems.ReferenceId
                            WHERE SaleInvoices.Id = @Id AND Payments.Cancelled = 0
                        )
                        SET @returnValueString = 'Please cancel related payments and try again.';

                    IF EXISTS(
                            SELECT *
                            FROM LessonSessions 
	                            LEFT JOIN SaleInvoiceitems ON SaleInvoiceitems.Id = LessonSessions.SaleInvoiceItems_Id
	                            LEFT JOIN SaleInvoices ON Saleinvoices.Id = SaleInvoiceitems.SaleInvoices_Id
                            WHERE SaleInvoices.Id = @Id AND LessonSessions.Deleted = 0
                        )
                        SET @returnValueString = 'Please cancel related lesson sessions and try again.';
   
                    IF @returnValueString IS NULL
                        BEGIN
                            UPDATE SaleInvoices 
                            SET
                                Cancelled = 1,
                                CancelNotes = @CancelNotes
                            WHERE SaleInvoices.Id = @Id;                    
                        END                    
                ", false, 
                true,
                new SqlQueryParameter(SaleInvoicesModel.COL_Id.Name, SqlDbType.UniqueIdentifier, Util.wrapNullable(Id)),
                new SqlQueryParameter(SaleInvoicesModel.COL_CancelNotes.Name, SqlDbType.VarChar, Util.wrapNullable(CancelNotes))
            );

            if (!string.IsNullOrEmpty(result.ValueString))
                return result.ValueString;
            else
            {
                ActivityLogsController.AddEditLog(db, Session, Id, string.Format(SaleInvoicesModel.COL_CancelNotes.LogDisplay, CancelNotes));
                db.SaveChanges();
                return null;
            }
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

            SaleInvoiceItemsController.add(Session, SaleInvoiceItems, model.Id);
        }

        /******************************************************************************************************************************************************/
    }
}