using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class SaleInvoiceItemsController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public static List<SaleInvoiceItemsModel> get(Guid? Id, Guid? SaleInvoices_Id, string SaleInvoices_IdList, Guid? Payments_Id)
        {
            string SaleInvoices_IdList_Clause = "";
            if (!string.IsNullOrEmpty(SaleInvoices_IdList))
                SaleInvoices_IdList_Clause = string.Format("AND SaleInvoices.Id IN ({0})", LIBWebMVC.UtilWebMVC.convertToSqlIdList(SaleInvoices_IdList));

            string sql = string.Format(@"
                    SELECT SaleInvoiceItems.*,
                        SaleInvoices.No AS SaleInvoices_No,
                        Customer_UserAccounts.Fullname AS Customer_UserAccounts_Name,
                        LessonPackages.Name AS LessonPackages_Name,
                        Services.Name AS Services_Name,
                        Products.Name AS Products_Name,
                        (SaleInvoiceItems.Qty * SaleInvoiceItems.Price) + COALESCE(SaleInvoiceItems.TravelCost,0) - COALESCE(SaleInvoiceItems.DiscountAmount,0) - COALESCE(SaleInvoiceItems.VouchersAmount,0) AS TotalAmount
                    FROM SaleInvoiceItems
                        LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                        LEFT JOIN LessonPackages ON LessonPackages.Id = SaleInvoiceItems.LessonPackages_Id
                        LEFT JOIN Services ON Services.Id = SaleInvoiceItems.Services_Id
                        LEFT JOIN Products ON Products.Id = SaleInvoiceItems.Products_Id
                        LEFT JOIN UserAccounts Customer_UserAccounts ON Customer_UserAccounts.Id = SaleInvoices.Customer_UserAccounts_Id
                    WHERE 1=1
						AND (@Id IS NULL OR SaleInvoiceItems.Id = @Id)
						AND (@Id IS NOT NULL OR (
                            (@SaleInvoices_Id IS NULL OR SaleInvoiceItems.SaleInvoices_Id = @SaleInvoices_Id)
                            AND (@Payments_Id IS NULL OR (SaleInvoiceItems.SaleInvoices_Id IN (
				                SELECT PaymentItems.ReferenceId
				                FROM PaymentItems
				                WHERE PaymentItems.Payments_Id = @Payments_Id
                            )))
                            {0}
                        ))
					ORDER BY SaleInvoiceItems.RowNo ASC
                ", SaleInvoices_IdList_Clause);

            return new DBContext().Database.SqlQuery<SaleInvoiceItemsModel>(sql,
                DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter("Payments_Id", Payments_Id),
                DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_SaleInvoices_Id.Name, SaleInvoices_Id)
            ).ToList();
        }

        public static void add(HttpSessionStateBase Session, List<SaleInvoiceItemsModel> SaleInvoiceItems, Guid SaleInvoices_Id)
        {
            DBContext db = new DBContext();
            int rowNo = 0;
            foreach(SaleInvoiceItemsModel saleInvoiceItem in SaleInvoiceItems)
            {
                db.Database.ExecuteSqlCommand(@"
                        INSERT INTO SaleInvoiceItems   (Id, Notes, RowNo, SaleInvoices_Id, Description, Qty, Price, DiscountAmount, Vouchers, VouchersAmount, VouchersName, Products_Id, Services_Id, LessonPackages_Id, SessionHours, SessionHours_Remaining, TravelCost, TutorTravelCost) 
                                                VALUES(@Id,@Notes,@RowNo,@SaleInvoices_Id,@Description,@Qty,@Price,@DiscountAmount,@Vouchers,@VouchersAmount,@VouchersName,@Products_Id,@Services_Id,@LessonPackages_Id,@SessionHours,@SessionHours_Remaining,@TravelCost,@TutorTravelCost);
                    ",
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Id.Name, Guid.NewGuid()),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Notes.Name, saleInvoiceItem.Notes),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_RowNo.Name, ++rowNo),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_SaleInvoices_Id.Name, SaleInvoices_Id),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Description.Name, saleInvoiceItem.Description),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Qty.Name, saleInvoiceItem.Qty),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Price.Name, saleInvoiceItem.Price),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_DiscountAmount.Name, saleInvoiceItem.DiscountAmount),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Vouchers.Name, saleInvoiceItem.Vouchers),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_VouchersName.Name, saleInvoiceItem.VouchersName),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_VouchersAmount.Name, saleInvoiceItem.VouchersAmount),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Products_Id.Name, saleInvoiceItem.Products_Id),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Services_Id.Name, saleInvoiceItem.Services_Id),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_LessonPackages_Id.Name, saleInvoiceItem.LessonPackages_Id),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_SessionHours.Name, saleInvoiceItem.SessionHours),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_SessionHours_Remaining.Name, saleInvoiceItem.SessionHours_Remaining),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_TravelCost.Name, saleInvoiceItem.TravelCost),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_TutorTravelCost.Name, saleInvoiceItem.TutorTravelCost)
                );

                if (saleInvoiceItem.Products_Id != null)
                {
                    int orderQty = saleInvoiceItem.Qty;
                    List<InventoryModel> inventoryList = InventoryController.get_by_Products_Id(Session, (Guid)saleInvoiceItem.Products_Id)
                        .Where(x => x.AvailableQty > 0)
                        .OrderBy(x=>x.ReceiveDate)
                        .ToList();

                    int qty = 0;
                    foreach (InventoryModel inventory in inventoryList)
                    {
                        if(inventory.AvailableQty >= orderQty)
                            qty = orderQty;
                        else
                            qty = inventory.AvailableQty;

                        SaleInvoiceItems_InventoryController.add(Session, db, new SaleInvoiceItems_InventoryModel
                        {
                            Id = Guid.NewGuid(),
                            SaleInvoiceItems_Id = saleInvoiceItem.Id,
                            Inventory_Id = inventory.Id,
                            Qty = qty
                        });

                        orderQty -= qty;
                        if (orderQty == 0)
                            break;
                    }
                }
            }
        }

        /******************************************************************************************************************************************************/
    }
}