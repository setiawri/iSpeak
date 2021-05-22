using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class PayrollPaymentItemsController : Controller
    {
        /* DATABASE METHODS ***********************************************************************************************************************************/

        public static List<PayrollPaymentItemsModel> get(Guid? Id, Guid? PayrollPayments_Id)
        {
            return new DBContext().Database.SqlQuery<PayrollPaymentItemsModel>(@"
                    SELECT PayrollPaymentItems.*,
                        UserAccounts.Fullname AS UserAccounts_Fullname
                    FROM PayrollPaymentItems
                        LEFT JOIN LessonSessions ON LessonSessions.PayrollPaymentItems_Id = PayrollPaymentItems.Id
                        LEFT JOIN SaleInvoiceItems ON SaleInvoiceItems.Id = LessonSessions.SaleInvoiceItems_Id
                        LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                        LEFT JOIN UserAccounts ON UserAccounts.Id = SaleInvoices.Customer_UserAccounts_Id
                    WHERE 1=1
						AND (@Id IS NULL OR PayrollPaymentItems.Id = @Id)
						AND (@Id IS NOT NULL OR (
                            (@PayrollPayments_Id IS NULL OR PayrollPaymentItems.PayrollPayments_Id = @PayrollPayments_Id)
                        ))
					ORDER BY PayrollPaymentItems.Timestamp ASC, UserAccounts.Fullname ASC
                ",
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_PayrollPayments_Id.Name, PayrollPayments_Id)
            ).ToList();
        }

        public static void add(DBContext db, Guid PayrollPayments_Id, PayrollPaymentItemsModel model)
        {
            db.Database.ExecuteSqlCommand(@"
                    INSERT INTO PayrollPaymentItems   (Id, PayrollPayments_Id, ReferenceId, Amount, DueBefore, DueAfter) 
                                        VALUES(@Id,@PayrollPayments_Id,@ReferenceId,@Amount,@DueBefore,@DueAfter);
                ",
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Id.Name, Guid.NewGuid())
            );
        }

        /******************************************************************************************************************************************************/
    }
}