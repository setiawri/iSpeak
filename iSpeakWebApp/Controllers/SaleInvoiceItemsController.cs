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

        public static List<SaleInvoiceItemsModel> get(Guid? Id, Guid? SaleInvoices_Id)
        {
            return new DBContext().Database.SqlQuery<SaleInvoiceItemsModel>(@"
                    SELECT SaleInvoiceItems.*,
                        SaleInvoices.No AS SaleInvoices_No,
                        LessonPackages.Name AS LessonPackages_Name,
                        Services.Name AS Services_Name,
                        Products.Name AS Products_Name,
                        COALESCE(SaleInvoiceItems_Vouchers.Amount,0) AS SaleInvoiceItems_Vouchers_Amount
                    FROM SaleInvoiceItems
                        LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                        LEFT JOIN LessonPackages ON LessonPackages.Id = SaleInvoiceItems.LessonPackages_Id
                        LEFT JOIN Services ON Services.Id = SaleInvoiceItems.Services_Id
                        LEFT JOIN Products ON Products.Id = SaleInvoiceItems.Products_Id
                        LEFT JOIN SaleInvoiceItems_Vouchers ON SaleInvoiceItems_Vouchers.Id = SaleInvoiceItems.SaleInvoiceItems_Vouchers_Id
                    WHERE 1=1
						AND (@Id IS NULL OR SaleInvoiceItems.Id = @Id)
						AND (@Id IS NOT NULL OR (
                            (@SaleInvoices_Id IS NULL OR SaleInvoiceItems.SaleInvoices_Id = @SaleInvoices_Id)
                        ))
					ORDER BY SaleInvoiceItems.RowNo ASC
                ",
                DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_SaleInvoices_Id.Name, SaleInvoices_Id)
            ).ToList();
        }

        public void update(SaleInvoiceItemsModel model, string log)
        {
            db.Database.ExecuteSqlCommand(@"
                UPDATE SaleInvoiceItems 
                SET
                    Name = @Name,
                    Active = @Active,
                    Notes = @Notes,
                    Branches_Id = @Branches_Id
                WHERE SaleInvoiceItems.Id = @Id;                
            ",
                DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Notes.Name, model.Notes)
            );

            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
            db.SaveChanges();
        }

        public void add(SaleInvoiceItemsModel model)
        {
            db.Database.ExecuteSqlCommand(@"
                INSERT INTO SaleInvoiceItems   (Id, Name, Active, Notes, Branches_Id) 
                                    VALUES(@Id,@Name,@Active,@Notes,@Branches_Id);
            ",
                DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(SaleInvoiceItemsModel.COL_Notes.Name, model.Notes)
            );

            ActivityLogsController.AddCreateLog(db, Session, model.Id);
            db.SaveChanges();
        }

        /******************************************************************************************************************************************************/
    }
}