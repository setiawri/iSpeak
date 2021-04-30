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
    public class SaleInvoiceItems_InventoryController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* METHODS ********************************************************************************************************************************************/

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public static List<SaleInvoiceItems_InventoryModel> get(HttpSessionStateBase Session, Guid? Id, Guid? SaleInvoiceItems_Id, Guid? Inventory_Id)
        {
            return new DBContext().Database.SqlQuery<SaleInvoiceItems_InventoryModel>(@"
                        SELECT SaleInvoiceItems_Inventory.*,
                            SaleInvoices.No AS SaleInvoices_No,
                            SaleInvoices.Cancelled AS SaleInvoices_Cancelled,
                            Inventory.BuyQty - (SUM(AdjustedQty.Qty) OVER(ORDER BY SaleInvoiceItems_Inventory.Inventory_Id ASC, SaleInvoices.Timestamp ASC)) AS Balance
                        FROM SaleInvoiceItems_Inventory
                            LEFT JOIN SaleInvoiceItems ON SaleInvoiceItems.Id = SaleInvoiceItems_Inventory.SaleInvoiceItems_Id
                            LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                            LEFT JOIN Inventory ON Inventory.Id = SaleInvoiceItems_Inventory.Inventory_Id
                            LEFT JOIN ( 
                                    SELECT SaleInvoiceItems_Inventory.Id, 
                                        CASE WHEN SaleInvoices.Cancelled = 1 THEN 0 ELSE SaleInvoiceItems_Inventory.Qty END AS Qty
                                    FROM SaleInvoiceItems_Inventory
                                        LEFT JOIN SaleInvoiceItems ON SaleInvoiceItems.Id = SaleInvoiceItems_Inventory.SaleInvoiceItems_Id
                                        LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                                ) AdjustedQty ON AdjustedQty.Id = SaleInvoiceItems_Inventory.Id
                        WHERE 1=1
							AND (@Id IS NULL OR SaleInvoiceItems_Inventory.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                SaleInvoices.Branches_Id = @Branches_Id
    							AND (@SaleInvoiceItems_Id IS NULL OR (SaleInvoiceItems_Inventory.SaleInvoiceItems_Id = @SaleInvoiceItems_Id))
    							AND (@Inventory_Id IS NULL OR (SaleInvoiceItems_Inventory.Inventory_Id = @Inventory_Id))
                            ))
                        ORDER BY SaleInvoiceItems_Inventory.Inventory_Id ASC, SaleInvoices.Timestamp ASC
                    ",
                    DBConnection.getSqlParameter(SaleInvoiceItems_InventoryModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(SaleInvoiceItems_InventoryModel.COL_SaleInvoiceItems_Id.Name, SaleInvoiceItems_Id),
                    DBConnection.getSqlParameter(SaleInvoiceItems_InventoryModel.COL_Inventory_Id.Name, Inventory_Id),
                    DBConnection.getSqlParameter("Branches_Id", Helper.getActiveBranchId(Session))
                ).ToList();
        }

        public static void add(HttpSessionStateBase Session, DBContext db, SaleInvoiceItems_InventoryModel model)
        {
            db.Database.ExecuteSqlCommand(@"
                INSERT INTO SaleInvoiceItems_Inventory   (Id, SaleInvoiceItems_Id, Inventory_Id, Qty) 
                                                  VALUES(@Id,@SaleInvoiceItems_Id,@Inventory_Id,@Qty);
            ",
                DBConnection.getSqlParameter(SaleInvoiceItems_InventoryModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(SaleInvoiceItems_InventoryModel.COL_SaleInvoiceItems_Id.Name, model.SaleInvoiceItems_Id),
                DBConnection.getSqlParameter(SaleInvoiceItems_InventoryModel.COL_Inventory_Id.Name, model.Inventory_Id),
                DBConnection.getSqlParameter(SaleInvoiceItems_InventoryModel.COL_Qty.Name, model.Qty)
            );

            ActivityLogsController.AddCreateLog(db, Session, model.Id);
            db.SaveChanges();
        }

        /******************************************************************************************************************************************************/
    }
}