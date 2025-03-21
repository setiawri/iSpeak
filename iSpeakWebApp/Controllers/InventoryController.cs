using System;
using System.Web;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    /*
     * Inventory is filtered by Franchise. The list is already filtered by Branch.
     */

    public class InventoryController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: Inventory
        [HttpGet]
        public ActionResult Index(int? rss, string FILTER_Keyword)
        {
            if (!UserAccountsController.getUserAccess(Session).Inventory_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                setViewBag(FILTER_Keyword);
                return View(get(Session, FILTER_Keyword));
            }
        }

        // POST: Inventory
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword)
        {
            setViewBag(FILTER_Keyword);
            return View(get(Session, FILTER_Keyword));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: Inventory/Create
        public ActionResult Create(string FILTER_Keyword)
        {
            if (!UserAccountsController.getUserAccess(Session).Inventory_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword);
            return View(new InventoryModel());
        }

        // POST: Inventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InventoryModel model, string FILTER_Keyword)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                model.Branches_Id = Helper.getActiveBranchId(Session);
                add(model);
                return RedirectToAction(nameof(Index), new { id = model.Id, FILTER_Keyword = FILTER_Keyword });
            }

            setViewBag(FILTER_Keyword);
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: Inventory/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword)
        {
            if (!UserAccountsController.getUserAccess(Session).Inventory_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword);
            return View(get(Session, (Guid)id));
        }

        // POST: Inventory/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InventoryModel modifiedModel, string FILTER_Keyword)
        {
            if (ModelState.IsValid)
            {
                InventoryModel originalModel = get(Session, modifiedModel.Id);

                string log = string.Empty;
                log = Helper.append(log, originalModel.Notes, modifiedModel.Notes, InventoryModel.COL_Notes.LogDisplay);
                log = Helper.append<BranchesModel>(log, originalModel.Branches_Id, modifiedModel.Branches_Id, InventoryModel.COL_Branches_Id.LogDisplay);
                log = Helper.append<ProductsModel>(log, originalModel.Products_Id, modifiedModel.Products_Id, InventoryModel.COL_Products_Id.LogDisplay);
                log = Helper.append(log, originalModel.ReceiveDate, modifiedModel.ReceiveDate, InventoryModel.COL_ReceiveDate.LogDisplay);
                log = Helper.append(log, originalModel.BuyQty, modifiedModel.BuyQty, InventoryModel.COL_BuyQty.LogDisplay);
                log = Helper.append<SuppliersModel>(log, originalModel.Suppliers_Id, modifiedModel.Suppliers_Id, InventoryModel.COL_Suppliers_Id.LogDisplay);
                log = Helper.append(log, originalModel.BuyPrice, modifiedModel.BuyPrice, InventoryModel.COL_BuyPrice.LogDisplay);

                if (!string.IsNullOrEmpty(log))
                {
                    update(modifiedModel, log);
                }

                return RedirectToAction(nameof(Index), new { FILTER_Keyword = FILTER_Keyword });
            }

            setViewBag(FILTER_Keyword);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ProductsController.setDropDownListViewBag(this, ProductsModel.COL_Name.Name);
            SuppliersController.setDropDownListViewBag(this);
        }

        public JsonResult Ajax_GetDetails(Guid id)
        {
            UserAccountRolesModel access = UserAccountsController.getUserAccess(Session);

            List<SaleInvoiceItems_InventoryModel> models = SaleInvoiceItems_InventoryController.get(Session, null, null, id);
            string content = string.Format(@"
                    <div class='table-responsive'>
                        <table class='table table-striped table-bordered'>
                            <thead>
                                <tr>
                                    <th>Invoice</th>
                                    <th class='text-right' style='width:70px;'>Qty</th>
                                    <th class='text-right' style='width:70px;'>Balance</th>
                                </tr>
                            </thead>
                            <tbody>
                ");

            string saleInvoiceLink;
            foreach (SaleInvoiceItems_InventoryModel model in models)
            {
                saleInvoiceLink = !access.SaleInvoices_View ? model.SaleInvoices_No :
                    string.Format("<a href='/SaleInvoices/Index?FILTER_chkDateFrom=false&FILTER_chkDateTo=false&FILTER_Keyword={0}' target='_blank'>{0}</a>", model.SaleInvoices_No);

                content += string.Format(@"
                            <tr>
                                <td>{0}{1}</td>
                                <td class='text-right'>{2:N0}</td>
                                <td class='text-right'>{3:N0}</td>
                            </tr>
                        ",
                        saleInvoiceLink,
                        !model.SaleInvoices_Cancelled ? string.Empty : "<span class='badge badge-warning ml-2' style='width:70px;'>CANCELLED</span>",
                        model.Qty,
                        model.Balance
                    );
            }

            content += string.Format(@"
                        </tbody></table></div>
                    "
                );

            return Json(new { content = content }, JsonRequestBehavior.AllowGet);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public static List<InventoryModel> get_by_Products_Id(HttpSessionStateBase Session, Guid Products_Id) { return get(Session, null, Products_Id, null); }
        public List<InventoryModel> get(HttpSessionStateBase Session, string FILTER_Keyword) { return get(Session, null, null, FILTER_Keyword); }
        public InventoryModel get(HttpSessionStateBase Session, Guid Id) { return get(Session, Id, null, null).FirstOrDefault(); }
        public static List<InventoryModel> get(HttpSessionStateBase Session) { return get(Session, null, null, null); }
        public static List<InventoryModel> get(HttpSessionStateBase Session, Guid? Id, Guid? Products_Id, string FILTER_Keyword)
        {
            return new DBContext().Database.SqlQuery<InventoryModel>(@"

                        SELECT Inventory.*,
                            Products.Name AS Products_Name,
                            Units.Name AS Units_Name,
                            Suppliers.Name AS Suppliers_Name,
                            ISNULL(Inventory.BuyQty,0) - ISNULL(SaleInvoiceItemsCount.SaleQty,0) + ISNULL(SaleReturnItemsCount.ReturnQty,0) AS AvailableQty,
                            ISNULL(GlobalInventory.AvailableQty,0) AS GlobalAvailableQty,
                            ROW_NUMBER() OVER (ORDER BY Inventory.ReceiveDate DESC) AS InitialRowNumber
                        FROM Inventory
                            LEFT JOIN Branches ON Branches.Id = Inventory.Branches_Id
                            LEFT JOIN Products ON Products.Id = Inventory.Products_Id
                            LEFT JOIN Units ON Units.Id = Products.Units_Id
                            LEFT JOIN Suppliers ON Suppliers.Id = Inventory.Suppliers_Id     
                            LEFT JOIN (
                                    SELECT Inventory.Id AS Inventory_Id, SUM(SaleInvoiceItems_Inventory.Qty) AS SaleQty
                                    FROM SaleInvoiceItems_Inventory
                                        LEFT JOIN SaleInvoiceItems ON SaleInvoiceItems.Id = SaleInvoiceItems_Inventory.SaleInvoiceItems_Id
                                        LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                                        LEFT JOIN Inventory ON Inventory.Id = SaleInvoiceItems_Inventory.Inventory_Id
                                        LEFT JOIN Products ON Products.Id = Inventory.Products_Id
                                    WHERE SaleInvoices.Cancelled = 0 
                                        AND SaleInvoices.Branches_Id = @Branches_Id
                                    GROUP BY Inventory.Id
                                ) SaleInvoiceItemsCount ON SaleInvoiceItemsCount.Inventory_Id = Inventory.Id
                            LEFT JOIN (
                                    SELECT Inventory.Id AS Inventory_Id, SUM(SaleInvoiceItems_Inventory.Qty) AS ReturnQty
                                    FROM SaleInvoiceItems_Inventory
                                        LEFT JOIN SaleInvoiceItems ON SaleInvoiceItems.Id = SaleInvoiceItems_Inventory.SaleInvoiceItems_Id
                                        LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                                        LEFT JOIN Inventory ON Inventory.Id = SaleInvoiceItems_Inventory.Inventory_Id
                                        LEFT JOIN Products ON Products.Id = Inventory.Products_Id
                                    WHERE SaleInvoices.Cancelled = 0 
                                        AND SaleInvoices.Branches_Id = @Branches_Id
                                        AND SaleInvoiceItems_Inventory.SaleInvoiceItems_Id IN (SELECT SaleReturnItems.SaleInvoiceItems_Id FROM SaleReturnItems)
                                    GROUP BY Inventory.Id
                                ) SaleReturnItemsCount ON SaleReturnItemsCount.Inventory_Id = Inventory.Id
                            LEFT JOIN (
                                    SELECT Products.Id AS Products_Id, 
                                        ISNULL(InventoryCount.BuyQty,0) - ISNULL(SaleInvoiceItemsCount.SaleQty,0) + ISNULL(SaleReturnItemsCount.ReturnQty,0) AS AvailableQty
                                    FROM Products
                                        LEFT JOIN (
                                                SELECT Inventory.Products_Id, SUM(Inventory.BuyQty) AS BuyQty
                                                FROM Inventory
                                                WHERE Inventory.Branches_Id = @Branches_Id
                                                GROUP BY Products_Id
                                            ) InventoryCount ON InventoryCount.Products_Id = Products.Id
                                        LEFT JOIN (
                                                SELECT Inventory.Products_Id, SUM(SaleInvoiceItems_Inventory.Qty) AS SaleQty
                                                FROM SaleInvoiceItems_Inventory
                                                    LEFT JOIN SaleInvoiceItems ON SaleInvoiceItems.Id = SaleInvoiceItems_Inventory.SaleInvoiceItems_Id
                                                    LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                                                    LEFT JOIN Inventory ON Inventory.Id = SaleInvoiceItems_Inventory.Inventory_Id
                                                    LEFT JOIN Products ON Products.Id = Inventory.Products_Id
                                                WHERE SaleInvoices.Cancelled = 0 AND SaleInvoices.Branches_Id = @Branches_Id
                                                GROUP BY Inventory.Products_Id
                                            ) SaleInvoiceItemsCount ON SaleInvoiceItemsCount.Products_Id = Products.Id
                                        LEFT JOIN (
                                                SELECT Products.Id AS Products_Id, SUM(SaleReturnItems.Qty) AS ReturnQty
                                                FROM SaleReturnItems
                                                    LEFT JOIN SaleInvoiceItems ON SaleInvoiceItems.Id = SaleReturnItems.SaleInvoiceItems_Id
                                                    LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                                                    LEFT JOIN Products ON Products.Id = SaleInvoiceItems.Products_Id
                                                WHERE SaleInvoices.Branches_Id = @Branches_Id
                                                GROUP BY Products.Id
                                            ) SaleReturnItemsCount ON SaleReturnItemsCount.Products_Id = Products.Id
                                ) GlobalInventory ON GlobalInventory.Products_Id = Inventory.Products_Id
                        WHERE 1=1
                            AND Inventory.Branches_Id = @Branches_Id
							AND (@Id IS NULL OR Inventory.Id = @Id)
							AND (@Id IS NOT NULL OR (
    							(@FILTER_Keyword IS NULL OR (Products.Name LIKE '%'+@FILTER_Keyword+'%'))
    							AND (@Products_Id IS NULL OR Inventory.Products_Id = @Products_Id)
                            ))
                            AND (Branches.Franchises_Id = @Franchises_Id)
						ORDER BY Inventory.ReceiveDate DESC
                    ",
                    DBConnection.getSqlParameter(InventoryModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(InventoryModel.COL_Branches_Id.Name, Helper.getActiveBranchId(Session)),
                    DBConnection.getSqlParameter(InventoryModel.COL_Products_Id.Name, Products_Id),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                    DBConnection.getSqlParameter("Franchises_Id", Helper.getActiveFranchiseId(Session))
                ).ToList();
        }

        public void add(InventoryModel model)
        {
            LIBWebMVC.WebDBConnection.Insert(db.Database, "Inventory",
                DBConnection.getSqlParameter(InventoryModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(InventoryModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(InventoryModel.COL_Branches_Id.Name, model.Branches_Id),
                DBConnection.getSqlParameter(InventoryModel.COL_Products_Id.Name, model.Products_Id),
                DBConnection.getSqlParameter(InventoryModel.COL_ReceiveDate.Name, model.ReceiveDate),
                DBConnection.getSqlParameter(InventoryModel.COL_BuyQty.Name, model.BuyQty),
                DBConnection.getSqlParameter(InventoryModel.COL_Suppliers_Id.Name, model.Suppliers_Id),
                DBConnection.getSqlParameter(InventoryModel.COL_BuyPrice.Name, model.BuyPrice)
            );

            ActivityLogsController.AddCreateLog(db, Session, model.Id);
        }

        public void update(InventoryModel model, string log)
        {
            LIBWebMVC.WebDBConnection.Update(db.Database, "Inventory",
                DBConnection.getSqlParameter(InventoryModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(InventoryModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(InventoryModel.COL_Branches_Id.Name, model.Branches_Id),
                DBConnection.getSqlParameter(InventoryModel.COL_Products_Id.Name, model.Products_Id),
                DBConnection.getSqlParameter(InventoryModel.COL_ReceiveDate.Name, model.ReceiveDate),
                DBConnection.getSqlParameter(InventoryModel.COL_BuyQty.Name, model.BuyQty),
                DBConnection.getSqlParameter(InventoryModel.COL_Suppliers_Id.Name, model.Suppliers_Id),
                DBConnection.getSqlParameter(InventoryModel.COL_BuyPrice.Name, model.BuyPrice)

            );

            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
        }

        /******************************************************************************************************************************************************/
    }
}