﻿using System;
using System.Web;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class InventoryController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: Inventory
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
                model.AvailableQty = model.BuyQty;
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
                log = Helper.append(log, originalModel.AvailableQty, modifiedModel.AvailableQty, InventoryModel.COL_AvailableQty.LogDisplay);
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

        public JsonResult GetDetails(Guid id)
        {
            UserAccountRolesModel access = UserAccountsController.getUserAccess(Session);

            List<SaleInvoiceItems_InventoryModel> models = SaleInvoiceItems_InventoryController.get(null, null, id);
            string content = string.Format(@"
                    <div class='table-responsive'>
                        <table class='table table-striped table-bordered'>
                            <thead>
                                <tr>
                                    <th>Invoice</th>
                                    <th class='text-right'>Qty</th>
                                    <th class='text-right'>Balance</th>
                                </tr>
                            </thead>
                            <tbody>
                ");

            string saleInvoiceLink;
            foreach (SaleInvoiceItems_InventoryModel model in models)
            {
                saleInvoiceLink = !access.SaleInvoices_View ? model.SaleInvoices_No :
                    string.Format("<a href='/SaleInvoices?FILTER_chkDateFrom=false&FILTER_chkDateTo=false&FILTER_Keyword={0}' target='_blank'>{0}</a>", model.SaleInvoices_No);

                content += string.Format(@"
                            <tr>
                                <td>{0}</td>
                                <td class='text-right'>{1:N0}</td>
                                <td class='text-right'>{2:N0}</td>
                            </tr>
                        ",
                        saleInvoiceLink,
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

        public List<InventoryModel> get(HttpSessionStateBase Session, string FILTER_Keyword) { return get(Session, null, FILTER_Keyword); }
        public InventoryModel get(HttpSessionStateBase Session, Guid Id) { return get(Session, Id, null).FirstOrDefault(); }
        public static List<InventoryModel> get(HttpSessionStateBase Session) { return get(Session, null, null); }
        public static List<InventoryModel> get(HttpSessionStateBase Session, Guid? Id, string FILTER_Keyword)
        {
            return new DBContext().Database.SqlQuery<InventoryModel>(@"
                        SELECT Inventory.*,
                            Products.Name AS Products_Name,
                            Suppliers.Name AS Suppliers_Name,
                            ISNULL(GlobalInventory.AvailableQty,0) AS GlobalAvailableQty,
                            ROW_NUMBER() OVER (ORDER BY Inventory.ReceiveDate DESC) AS InitialRowNumber
                        FROM Inventory
                            LEFT JOIN Products ON Products.Id = Inventory.Products_Id
                            LEFT JOIN Suppliers ON Suppliers.Id = Inventory.Suppliers_Id
                            LEFT JOIN (
                                    SELECT Inventory.Products_Id, SUM(Inventory.AvailableQty) AS AvailableQty
                                    FROM Inventory
                                    WHERE Inventory.Branches_Id = @Branches_Id
                                    GROUP BY Inventory.Products_Id
                                ) GlobalInventory ON GlobalInventory.Products_Id = Inventory.Products_Id
                        WHERE 1=1
                            AND Inventory.Branches_Id = @Branches_Id
							AND (@Id IS NULL OR Inventory.Id = @Id)
							AND (@Id IS NOT NULL OR (
    							(@FILTER_Keyword IS NULL OR (Products.Name LIKE '%'+@FILTER_Keyword+'%'))
                            ))
						ORDER BY Inventory.ReceiveDate DESC
                    ",
                    DBConnection.getSqlParameter(InventoryModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(InventoryModel.COL_Branches_Id.Name, Helper.getActiveBranchId(Session)),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
                ).ToList();
        }

        public void add(InventoryModel model)
        {
            db.Database.ExecuteSqlCommand(@"
                INSERT INTO Inventory   (Id, Notes, Branches_Id, Products_Id, ReceiveDate, BuyQty, AvailableQty, BuyPrice, Suppliers_Id) 
                                 VALUES(@Id,@Notes,@Branches_Id,@Products_Id,@ReceiveDate,@BuyQty,@AvailableQty,@BuyPrice,@Suppliers_Id);
            ",
                DBConnection.getSqlParameter(InventoryModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(InventoryModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(InventoryModel.COL_Branches_Id.Name, model.Branches_Id),
                DBConnection.getSqlParameter(InventoryModel.COL_Products_Id.Name, model.Products_Id),
                DBConnection.getSqlParameter(InventoryModel.COL_ReceiveDate.Name, model.ReceiveDate),
                DBConnection.getSqlParameter(InventoryModel.COL_BuyQty.Name, model.BuyQty),
                DBConnection.getSqlParameter(InventoryModel.COL_AvailableQty.Name, model.AvailableQty),
                DBConnection.getSqlParameter(InventoryModel.COL_Suppliers_Id.Name, model.Suppliers_Id),
                DBConnection.getSqlParameter(InventoryModel.COL_BuyPrice.Name, model.BuyPrice)
            );

            ActivityLogsController.AddCreateLog(db, Session, model.Id);
            db.SaveChanges();
        }

        public void update(InventoryModel model, string log)
        {
            db.Database.ExecuteSqlCommand(@"
                UPDATE Inventory 
                SET
                    Notes = @Notes,
                    Branches_Id = @Branches_Id,
                    Products_Id = @Products_Id,
                    ReceiveDate = @ReceiveDate,
                    BuyQty = @BuyQty,
                    AvailableQty = @AvailableQty,
                    Suppliers_Id = @Suppliers_Id,
                    BuyPrice = @BuyPrice
                WHERE Inventory.Id = @Id;                
            ",
                DBConnection.getSqlParameter(InventoryModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(InventoryModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(InventoryModel.COL_Branches_Id.Name, model.Branches_Id),
                DBConnection.getSqlParameter(InventoryModel.COL_Products_Id.Name, model.Products_Id),
                DBConnection.getSqlParameter(InventoryModel.COL_ReceiveDate.Name, model.ReceiveDate),
                DBConnection.getSqlParameter(InventoryModel.COL_BuyQty.Name, model.BuyQty),
                DBConnection.getSqlParameter(InventoryModel.COL_AvailableQty.Name, model.AvailableQty),
                DBConnection.getSqlParameter(InventoryModel.COL_Suppliers_Id.Name, model.Suppliers_Id),
                DBConnection.getSqlParameter(InventoryModel.COL_BuyPrice.Name, model.BuyPrice)
            );

            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
            db.SaveChanges();
        }

        /******************************************************************************************************************************************************/
    }
}