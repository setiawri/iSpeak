using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class SaleInvoicesController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: SaleInvoices
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Cancelled, int? FILTER_Approved, bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (!UserAccountsController.getUserAccess(Session).SaleInvoices_View)
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

        // POST: SaleInvoices
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Cancelled, int? FILTER_Approved, bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            setViewBag(FILTER_Keyword, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            return View(get(FILTER_Keyword, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: SaleInvoices/Create
        public ActionResult Create(string FILTER_Keyword, int? FILTER_Cancelled, int? FILTER_Approved, bool? FILTER_chkDateFrom, DateTime FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (!UserAccountsController.getUserAccess(Session).SaleInvoices_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            return View(new SaleInvoicesModel());
        }

        // POST: SaleInvoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SaleInvoicesModel model, string FILTER_Keyword, int? FILTER_Cancelled, int? FILTER_Approved, bool? FILTER_chkDateFrom, DateTime FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (ModelState.IsValid)
            {
                add(model);
                return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword });
            }

            setViewBag(FILTER_Keyword, FILTER_Cancelled, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            return View(model);
        }

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Cancelled, int? FILTER_Approved, bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
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
                decimal voucher = model.SaleInvoiceItems_Vouchers_Amount;

                string subtractions = "";
                if (voucher != 0)
                    subtractions = Util.append(subtractions, string.Format("Voucher: -{0:N0}", voucher), "<br/>");
                if (model.DiscountAmount != 0)
                    subtractions = Util.append(subtractions, string.Format("Discount: -{0:N0}", model.DiscountAmount), "<br/>");
                if (!string.IsNullOrEmpty(subtractions))
                    subtractions += "<br/>";

                content += string.Format(@"
                            <tr>
                                <td class='align-top' style='width:10px;'>{0}</td>
                                <td class='align-top'>{1}<br/><strong>Available Hours:</strong> {7:N0}{2}</td>
                                <td class='align-top text-right'>
                                    Qty: {3:N0} @{4:N0}
                                    <br/>Travel: {5:N0}{6}
                                </td>
                                <td class='align-top text-right'>
                                    {8}
                                    <strong>{9:N0}</strong>
                                </td>
                            </tr>
                        ",
                        access.SaleInvoices_TutorTravelCost_View ? string.Format("<a href='{0}Logs/Index/{1}?ctrl=Sale&table=SaleInvoiceItems&header={1}' target='_blank'>{2}</a>", Url.Content("~"), model.Id, model.RowNo) : model.RowNo.ToString(),
                        model.Description,
                        !string.IsNullOrWhiteSpace(model.Notes) ? string.Format("<br/>Notes: {0}", model.Notes) : "",
                        model.Qty,
                        model.Price,
                        model.TravelCost,
                        access.SaleInvoices_TutorTravelCost_View ? string.Format(" (Tutor: {0:N0})", model.TutorTravelCost) : "",
                        model.SessionHours_Remaining,
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

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public List<SaleInvoicesModel> get(string FILTER_Keyword, int? FILTER_Cancelled, int? FILTER_Approved, bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo) 
        { return get(Session, null, FILTER_Keyword, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo, FILTER_Cancelled, FILTER_Approved); }
        public SaleInvoicesModel get(Guid Id) { return get(Session, Id, null, false, null, false, null, null, null).FirstOrDefault(); }
        public static List<SaleInvoicesModel> get(HttpSessionStateBase Session, Guid? Id, string FILTER_Keyword, bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo, 
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
                DBConnection.getSqlParameter("FILTER_DateFrom", FILTER_DateFrom),
                DBConnection.getSqlParameter("FILTER_DateTo", Util.getAsEndDate(FILTER_DateTo)),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Branches_Id.Name, Branches_Id),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Cancelled.Name, Cancelled),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_IsChecked.Name, IsChecked)
            ).ToList();
        }

        public void update(SaleInvoicesModel model, string log)
        {
            db.Database.ExecuteSqlCommand(@"
                UPDATE SaleInvoices 
                SET
                    Name = @Name,
                    Active = @Active,
                    Notes = @Notes,
                    Branches_Id = @Branches_Id
                WHERE SaleInvoices.Id = @Id;                
            ",
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Branches_Id.Name, model.Branches_Id)
            );

            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
            db.SaveChanges();
        }

        public void add(SaleInvoicesModel model)
        {
            db.Database.ExecuteSqlCommand(@"
                INSERT INTO SaleInvoices   (Id, Name, Active, Notes, Branches_Id) 
                                    VALUES(@Id,@Name,@Active,@Notes,@Branches_Id);
            ",
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(SaleInvoicesModel.COL_Branches_Id.Name, model.Branches_Id)
            );

            ActivityLogsController.AddCreateLog(db, Session, model.Id);
            db.SaveChanges();
        }

        /******************************************************************************************************************************************************/
    }
}