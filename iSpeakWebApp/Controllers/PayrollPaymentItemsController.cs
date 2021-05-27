using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class PayrollPaymentItemsController : Controller
    {
        public JsonResult GetDetails(Guid id, DateTime DatePeriod)
        {
            string content = "";

            List<PayrollPaymentItemsModel> models = get(Session, id, DatePeriod);

            if(models.Count > 0)
            {
                Guid Tutor_UserAccounts_Id = models[0].UserAccounts_Id_TEMP;
                string Tutor_UserAccounts_Fullname = models[0].UserAccounts_Fullname;

                List<PayrollPaymentItemsModel> combinedModels = combineClassSesions(models);
                    
                content = string.Format(@"
                        <div class='table-responsive'>
                            <table class='table table-sm table-striped table-bordered'>
                                <thead>
                                    <tr>
                                        <th style='width:140px;'>Date</th>
                                        <th>Description</th>
                                        <th class='text-right' style='width:40px;'>Hour</th>
                                        <th class='text-right' style='width:40px;'>Rate</th>
                                        <th class='text-right' style='width:40px;'>Travel</th>
                                        <th class='text-right' style='width:40px;'>Amount</th>
                                        <th class='text-center' style='width:20px;'>Paid</th>
                                    </tr>
                                </thead>
                                <tbody>
                    ");

                foreach (PayrollPaymentItemsModel model in combinedModels)
                {
                    content += string.Format(@"
                                <tr>
                                    <td class='align-top'>{0:dd/MM/yy HH:mm}</td>
                                    <td class='align-top'>{1}</td>
                                    <td class='align-top text-right'>{2:N2}</td>
                                    <td class='align-top text-right'>{3:N2}</td>
                                    <td class='align-top text-right'>{4:N2}</td>
                                    <td class='align-top text-right'>{5:N2}</td>
                                    <td class='align-top text-center'>{6}</td>
                                </tr>
                            ",
                            model.Timestamp,
                            model.Description,
                            model.Hour,
                            model.HourlyRate,
                            model.TutorTravelCost,
                            model.Amount,
                            model.PayrollPayments_Id == null ? "<span class='text-danger'><i class='icon-cancel-circle2'></i></span>" : "<span class='text-primary'><i class='icon-checkmark'></i></span>"
                        );
                }

                decimal due = combinedModels.Where(x => x.PayrollPayments_Id == null).Sum(x => x.Amount);
                content += string.Format(@"
                            </tbody></table></div>

                            <div class='mt-2 float-right'>
                                <div class='h3 ml-2'>TOTAL: {0:N2}</div>
                            </div>
                            <div class='mt-2 row'>
                                <div class='h3 ml-2 font-weight-bold'>DUE: {1:N2}</div>
                                <div><button type='button' class='btn btn-success mx-2' data-toggle='modal' data-target='#modal_payment' {2}"
                        + "onclick=\"ClosePayrollItemsDialog('{3}','{4}',{1})\""
                        + @"><i class='icon-checkmark3 mr-2'></i>CREATE PAYMENT</button></div>
                            </div>
                        ",
                        combinedModels.Sum(x=>x.Amount),
                        due,
                        due > 0 ? "" : "disabled='true'",
                        Tutor_UserAccounts_Id,
                        Tutor_UserAccounts_Fullname
                    );
            }

            return Json(new { content = content }, JsonRequestBehavior.AllowGet);
        }

        public static List<PayrollPaymentItemsModel> combineClassSesions(List<PayrollPaymentItemsModel> models)
        {
            List<PayrollPaymentItemsModel> combinedModels = new List<PayrollPaymentItemsModel>();

            PayrollPaymentItemsModel existingModel;
            foreach (PayrollPaymentItemsModel model in models)
            {
                existingModel = combinedModels.Find(x => x.Id == model.Id);
                if (existingModel != null)
                    existingModel.Description = Util.append(existingModel.Description, model.Student_UserAccounts_FirstName, ", ");
                else
                {
                    if (!string.IsNullOrEmpty(model.Student_UserAccounts_FirstName))
                        model.Description = Util.append(model.Description, model.Student_UserAccounts_FirstName, ",");

                    combinedModels.Add(model);
                }
            }

            return combinedModels;
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public static List<PayrollPaymentItemsModel> get(HttpSessionStateBase Session, Guid? UserAccounts_Id, DateTime? DatePeriod) { return get(Session, null, null, UserAccounts_Id, DatePeriod); }
        public static List<PayrollPaymentItemsModel> get(HttpSessionStateBase Session, Guid? Id, Guid? PayrollPayments_Id, Guid? UserAccounts_Id, DateTime? DatePeriod)
        {
            return new DBContext().Database.SqlQuery<PayrollPaymentItemsModel>(@"
                    SELECT PayrollPaymentItems.*,
                        UserAccounts.Fullname AS UserAccounts_Fullname,
                        Student_UserAccounts.Fullname AS Student_UserAccounts_Fullname,
                        CASE CHARINDEX(' ', Student_UserAccounts.Fullname, 1)
                             WHEN 0 THEN Student_UserAccounts.Fullname
                             ELSE SUBSTRING(Student_UserAccounts.Fullname, 1, CHARINDEX(' ', Student_UserAccounts.Fullname, 1) - 1)
                        END AS Student_UserAccounts_FirstName
                    FROM PayrollPaymentItems
                        LEFT JOIN LessonSessions ON LessonSessions.PayrollPaymentItems_Id = PayrollPaymentItems.Id
                        LEFT JOIN SaleInvoiceItems ON SaleInvoiceItems.Id = LessonSessions.SaleInvoiceItems_Id
                        LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                        LEFT JOIN UserAccounts ON UserAccounts.Id = PayrollPaymentItems.UserAccounts_Id_TEMP
						LEFT JOIN UserAccounts Student_UserAccounts ON Student_UserAccounts.Id = SaleInvoices.Customer_UserAccounts_Id
                    WHERE 1=1
						AND (@Id IS NULL OR PayrollPaymentItems.Id = @Id)
						AND (@Id IS NOT NULL OR (
                            PayrollPaymentItems.Branches_Id = @Branches_Id
                            AND (@PayrollPayments_Id IS NULL OR PayrollPaymentItems.PayrollPayments_Id = @PayrollPayments_Id)
                            AND (@UserAccounts_Id_TEMP IS NULL OR PayrollPaymentItems.UserAccounts_Id_TEMP = @UserAccounts_Id_TEMP)
                            AND (@DatePeriod IS NULL OR (MONTH(PayrollPaymentItems.Timestamp) = MONTH(@DatePeriod) AND YEAR(PayrollPaymentItems.Timestamp) = YEAR(@DatePeriod)))
                        ))
					ORDER BY PayrollPaymentItems.Timestamp ASC, UserAccounts.Fullname ASC
                ",
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Branches_Id.Name, Helper.getActiveBranchId(Session)),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_PayrollPayments_Id.Name, PayrollPayments_Id),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_UserAccounts_Id_TEMP.Name, UserAccounts_Id),
                DBConnection.getSqlParameter("DatePeriod", DatePeriod)
            ).ToList();
        }

        public static void add(PayrollPaymentItemsModel model)
        {
            new DBContext().Database.ExecuteSqlCommand(@"
                    INSERT INTO PayrollPaymentItems (Id, PayrollPayments_Id, Timestamp, Description, Hour, HourlyRate, TutorTravelCost, Amount, UserAccounts_Id_TEMP, CancelNotes, Branches_Id) 
                                             VALUES(@Id,@PayrollPayments_Id,@Timestamp,@Description,@Hour,@HourlyRate,@TutorTravelCost,@Amount,@UserAccounts_Id_TEMP,@CancelNotes,@Branches_Id);
                ",
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_PayrollPayments_Id.Name, model.PayrollPayments_Id),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Timestamp.Name, model.Timestamp),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Description.Name, model.Description),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Hour.Name, model.Hour),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_HourlyRate.Name, model.HourlyRate),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_TutorTravelCost.Name, model.TutorTravelCost),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Amount.Name, model.Amount),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_UserAccounts_Id_TEMP.Name, model.UserAccounts_Id_TEMP),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_CancelNotes.Name, model.CancelNotes),
                DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Branches_Id.Name, model.Branches_Id)
            );
        }

        public static void update(DBContext db, HttpSessionStateBase Session, Guid? PayrollPayments_Id, List<PayrollPaymentItemsModel> models)
        {
            foreach (PayrollPaymentItemsModel model in models)
            {
                db.Database.ExecuteSqlCommand(@"
                    UPDATE PayrollPaymentItems   
                        SET PayrollPayments_Id = @PayrollPayments_Id
                    WHERE Id = @Id;
                ",
                    DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_Id.Name, model.Id),
                    DBConnection.getSqlParameter(PayrollPaymentItemsModel.COL_PayrollPayments_Id.Name, PayrollPayments_Id)
                );
            }
        }

        /******************************************************************************************************************************************************/
    }
}