using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class PaymentItemsController : Controller
    {
        /* DATABASE METHODS ***********************************************************************************************************************************/

        public static List<PaymentItemsModel> get(Guid? Id, Guid? Payments_Id)
        {
            return new DBContext().Database.SqlQuery<PaymentItemsModel>(@"
                    SELECT PaymentItems.*,
                        Payments.No AS Payments_No,
                        SaleInvoices.No AS SaleInvoices_No
                    FROM PaymentItems
                        LEFT JOIN Payments ON Payments.Id = PaymentItems.Payments_Id
                        LEFT JOIN SaleInvoices ON SaleInvoices.Id = PaymentItems.ReferenceId
                    WHERE 1=1
						AND (@Id IS NULL OR PaymentItems.Id = @Id)
						AND (@Id IS NOT NULL OR (
                            (@Payments_Id IS NULL OR PaymentItems.Payments_Id = @Payments_Id)
                        ))
					ORDER BY Payments.No ASC
                ",
                DBConnection.getSqlParameter(PaymentItemsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(PaymentItemsModel.COL_Payments_Id.Name, Payments_Id)
            ).ToList();
        }

        //public static void add(List<PaymentItemsModel> models, Guid Payments_Id)
        //{
        //    DBContext db = new DBContext();
        //    int rowNo = 0;
        //    foreach(PaymentItemsModel model in models)
        //    {
        //        db.Database.ExecuteSqlCommand(@"
        //                INSERT INTO PaymentItems   (Id, Notes, RowNo, Payments_Id, Description, Qty, Price, DiscountAmount, Vouchers, VouchersAmount, VouchersName, Products_Id, Services_Id, LessonPackages_Id, SessionHours, SessionHours_Remaining, TravelCost, TutorTravelCost) 
        //                                        VALUES(@Id,@Notes,@RowNo,@Payments_Id,@Description,@Qty,@Price,@DiscountAmount,@Vouchers,@VouchersAmount,@VouchersName,@Products_Id,@Services_Id,@LessonPackages_Id,@SessionHours,@SessionHours_Remaining,@TravelCost,@TutorTravelCost);
        //            ",
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_Id.Name, Guid.NewGuid()),
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_Notes.Name, model.Notes),
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_RowNo.Name, ++rowNo),
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_Payments_Id.Name, Payments_Id),
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_Description.Name, model.Description),
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_Qty.Name, model.Qty),
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_Price.Name, model.Price),
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_DiscountAmount.Name, model.DiscountAmount),
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_Vouchers.Name, model.Vouchers),
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_VouchersName.Name, model.VouchersName),
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_VouchersAmount.Name, model.VouchersAmount),
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_Products_Id.Name, model.Products_Id),
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_Services_Id.Name, model.Services_Id),
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_LessonPackages_Id.Name, model.LessonPackages_Id),
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_SessionHours.Name, model.SessionHours),
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_SessionHours_Remaining.Name, model.SessionHours_Remaining),
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_TravelCost.Name, model.TravelCost),
        //            DBConnection.getSqlParameter(PaymentItemsModel.COL_TutorTravelCost.Name, model.TutorTravelCost)
        //        );
        //    }
        //}

        /******************************************************************************************************************************************************/
    }
}