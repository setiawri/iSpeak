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
                        SaleInvoices.No AS SaleInvoices_No,
                        SaleInvoices.Amount AS SaleInvoices_Amount
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

        public static void add(DBContext db, Guid Payments_Id, PaymentItemsModel model)
        {
            db.Database.ExecuteSqlCommand(@"
                    INSERT INTO PaymentItems   (Id, Payments_Id, ReferenceId, Amount, DueBefore, DueAfter) 
                                        VALUES(@Id,@Payments_Id,@ReferenceId,@Amount,@DueBefore,@DueAfter);
                ",
                DBConnection.getSqlParameter(PaymentItemsModel.COL_Id.Name, Guid.NewGuid()),
                DBConnection.getSqlParameter(PaymentItemsModel.COL_Payments_Id.Name, Payments_Id),
                DBConnection.getSqlParameter(PaymentItemsModel.COL_ReferenceId.Name, model.ReferenceId),
                DBConnection.getSqlParameter(PaymentItemsModel.COL_Amount.Name, model.Amount),
                DBConnection.getSqlParameter(PaymentItemsModel.COL_DueBefore.Name, model.DueBefore),
                DBConnection.getSqlParameter(PaymentItemsModel.COL_DueAfter.Name, model.DueAfter)
            );
        }

        /******************************************************************************************************************************************************/
    }
}