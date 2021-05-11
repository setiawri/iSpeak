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

        /* METHODS ********************************************************************************************************************************************/

        public JsonResult GetActiveLessonPackages(Guid Customer_UserAccounts_Id, int? hasLessonHours)
        {
            SelectList content = new SelectList(getActiveLessonPackages(Customer_UserAccounts_Id, hasLessonHours), SaleInvoiceItemsModel.COL_Id.Name, SaleInvoiceItemsModel.COL_DDLDescription.Name);

            return Json(new { content = content }, JsonRequestBehavior.AllowGet);
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public static List<SaleInvoiceItemsModel> getActiveLessonPackages(Guid Customer_UserAccounts_Id, int? hasLessonHours) { return get(null, null, null, null, Customer_UserAccounts_Id, hasLessonHours, null); }
        public static List<SaleInvoiceItemsModel> get_by_SaleInvoices_Id(Guid SaleInvoices_Id) { return get(null, SaleInvoices_Id, null, null, null, null, null); }
        public static List<SaleInvoiceItemsModel> get_by_IdList(string IdList) { return get(null, null, null, null, null, null, IdList); }
        public static List<SaleInvoiceItemsModel> get(Guid? Id, Guid? SaleInvoices_Id, string SaleInvoices_IdList, Guid? Payments_Id, Guid? Customer_UserAccounts_Id, int? hasLessonHours, string IdList)
        {
            string IdList_Clause = "";
            if (!string.IsNullOrEmpty(IdList))
                IdList_Clause = string.Format("AND SaleInvoiceItems.Id IN ({0})", LIBWebMVC.UtilWebMVC.convertToSqlIdList(IdList));

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
                        (SaleInvoiceItems.Qty * SaleInvoiceItems.Price) + COALESCE(SaleInvoiceItems.TravelCost,0) - COALESCE(SaleInvoiceItems.DiscountAmount,0) - COALESCE(SaleInvoiceItems.VouchersAmount,0) AS TotalAmount,
                        SaleInvoiceItems.Description + ', Available: ' + FORMAT(SaleInvoiceItems.SessionHours_Remaining,'N2') + ' hours' AS DDLDescription
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
                            AND (@Customer_UserAccounts_Id IS NULL OR SaleInvoices.Customer_UserAccounts_Id = @Customer_UserAccounts_Id)
                            AND (@hasLessonHours IS NULL OR SaleInvoiceItems.SessionHours_Remaining > 0)
                            {0}{1}
                        ))
					ORDER BY SaleInvoiceItems.RowNo ASC
                ", IdList_Clause, SaleInvoices_IdList_Clause);

            return new DBContext().Database.SqlQuery<SaleInvoiceItemsModel>(sql,
                DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter("Payments_Id", Payments_Id),
                DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_SaleInvoices_Id.Name, SaleInvoices_Id),
                DBConnection.getSqlParameter("Customer_UserAccounts_Id", Customer_UserAccounts_Id),
                DBConnection.getSqlParameter("hasLessonHours", hasLessonHours)
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

        public static void update_SessionHours_Remaining(DBContext db, HttpSessionStateBase Session, Guid Id, decimal value, string log)
        {
            db.Database.ExecuteSqlCommand(@"
                UPDATE SaleInvoiceItems 
                SET
                    SessionHours_Remaining = @SessionHours_Remaining
                WHERE SaleInvoiceItems.Id = @Id;                
            ",
                DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_SessionHours_Remaining.Name, value)
            );

            ActivityLogsController.AddEditLog(db, Session, Id, log);
        }

        /******************************************************************************************************************************************************/
    }
}