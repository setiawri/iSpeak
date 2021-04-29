using System;
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

        public static List<SaleInvoiceItems_InventoryModel> get(Guid? Id, Guid? SaleInvoiceItems_Id, Guid? Inventory_Id)
        {
            return new DBContext().Database.SqlQuery<SaleInvoiceItems_InventoryModel>(@"
                        SELECT SaleInvoiceItems_Inventory.*,
                            SaleInvoices.No AS SaleInvoices_No,
                            Inventory.BuyQty - (SUM(SaleInvoiceItems_Inventory.Qty) OVER(ORDER BY SaleInvoiceItems_Inventory.Inventory_Id ASC, SaleInvoices.Timestamp ASC)) AS Balance
                        FROM SaleInvoiceItems_Inventory
                            LEFT JOIN SaleInvoiceItems ON SaleInvoiceItems.Id = SaleInvoiceItems_Inventory.SaleInvoiceItems_Id
                            LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                            LEFT JOIN Inventory ON Inventory.Id = SaleInvoiceItems_Inventory.Inventory_Id
                        WHERE 1=1
							AND (@Id IS NULL OR SaleInvoiceItems_Inventory.Id = @Id)
							AND (@Id IS NOT NULL OR (
    							(@SaleInvoiceItems_Id IS NULL OR (SaleInvoiceItems_Inventory.SaleInvoiceItems_Id = @SaleInvoiceItems_Id))
    							AND (@Inventory_Id IS NULL OR (SaleInvoiceItems_Inventory.Inventory_Id = @Inventory_Id))
                            ))
                        ORDER BY SaleInvoiceItems_Inventory.Inventory_Id ASC, SaleInvoices.Timestamp ASC
                    ",
                    DBConnection.getSqlParameter(SaleInvoiceItems_InventoryModel.COL_Id.Name, Id),
                    DBConnection.getSqlParameter(SaleInvoiceItems_InventoryModel.COL_SaleInvoiceItems_Id.Name, SaleInvoiceItems_Id),
                    DBConnection.getSqlParameter(SaleInvoiceItems_InventoryModel.COL_Inventory_Id.Name, Inventory_Id)
                ).ToList();
        }

        public static void add(DBContext db, SaleInvoiceItems_InventoryModel model)
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