using System;
using System.Data;
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

        public static List<SaleInvoiceItemsModel> get(Guid? Id, Guid? SaleInvoices_Id, string IdList)
        {
            string IdListClause = "";
            if (!string.IsNullOrEmpty(IdList))
                IdListClause = string.Format("AND SaleInvoices.Id IN ({0})", LIBWebMVC.UtilWebMVC.convertToSqlIdList(IdList));

            string sql = string.Format(@"
                    SELECT SaleInvoiceItems.*,
                        SaleInvoices.No AS SaleInvoices_No,
                        LessonPackages.Name AS LessonPackages_Name,
                        Services.Name AS Services_Name,
                        Products.Name AS Products_Name,
                        (SaleInvoiceItems.Qty * SaleInvoiceItems.Price) - COALESCE(SaleInvoiceItems.DiscountAmount,0) - COALESCE(SaleInvoiceItems.VouchersAmount,0) AS TotalAmount
                    FROM SaleInvoiceItems
                        LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                        LEFT JOIN LessonPackages ON LessonPackages.Id = SaleInvoiceItems.LessonPackages_Id
                        LEFT JOIN Services ON Services.Id = SaleInvoiceItems.Services_Id
                        LEFT JOIN Products ON Products.Id = SaleInvoiceItems.Products_Id
                    WHERE 1=1
						AND (@Id IS NULL OR SaleInvoiceItems.Id = @Id)
						AND (@Id IS NOT NULL OR (
                            (@SaleInvoices_Id IS NULL OR SaleInvoiceItems.SaleInvoices_Id = @SaleInvoices_Id)
                            {0}
                        ))
					ORDER BY SaleInvoiceItems.RowNo ASC
                ", IdListClause);

            return new DBContext().Database.SqlQuery<SaleInvoiceItemsModel>(sql,
                DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_SaleInvoices_Id.Name, SaleInvoices_Id)
            ).ToList();
        }

        public static void add(List<SaleInvoiceItemsModel> models, Guid SaleInvoices_Id)
        {
            DBContext db = new DBContext();
            int rowNo = 0;
            foreach(SaleInvoiceItemsModel model in models)
            {
                db.Database.ExecuteSqlCommand(@"
                        INSERT INTO SaleInvoiceItems   (Id, Notes, RowNo, SaleInvoices_Id, Description, Qty, Price, DiscountAmount, Vouchers, VouchersAmount, VouchersName, Products_Id, Services_Id, LessonPackages_Id, SessionHours, SessionHours_Remaining, TravelCost, TutorTravelCost) 
                                                VALUES(@Id,@Notes,@RowNo,@SaleInvoices_Id,@Description,@Qty,@Price,@DiscountAmount,@Vouchers,@VouchersAmount,@VouchersName,@Products_Id,@Services_Id,@LessonPackages_Id,@SessionHours,@SessionHours_Remaining,@TravelCost,@TutorTravelCost);
                    ",
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Id.Name, Guid.NewGuid()),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Notes.Name, model.Notes),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_RowNo.Name, ++rowNo),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_SaleInvoices_Id.Name, SaleInvoices_Id),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Description.Name, model.Description),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Qty.Name, model.Qty),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Price.Name, model.Price),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_DiscountAmount.Name, model.DiscountAmount),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Vouchers.Name, model.Vouchers),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_VouchersName.Name, model.VouchersName),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_VouchersAmount.Name, model.VouchersAmount),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Products_Id.Name, model.Products_Id),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Services_Id.Name, model.Services_Id),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_LessonPackages_Id.Name, model.LessonPackages_Id),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_SessionHours.Name, model.SessionHours),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_SessionHours_Remaining.Name, model.SessionHours_Remaining),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_TravelCost.Name, model.TravelCost),
                    DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_TutorTravelCost.Name, model.TutorTravelCost)
                );
            }
        }

        /******************************************************************************************************************************************************/
    }
}