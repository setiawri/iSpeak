using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using LIBUtil;

namespace iSpeakWebApp.Controllers
{
    public class PettyCashRecordsController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: PettyCashRecords
        public ActionResult Index(int? rss, string FILTER_Keyword, int? FILTER_Approved, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (!UserAccountsController.getUserAccess(Session).PettyCashRecords_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (rss != null && FILTER_DateFrom == null)
            {
                FILTER_chkDateFrom = true;
                FILTER_DateFrom = DateTime.Today.AddMonths(-1);
            }

            setViewBag(FILTER_Keyword, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                return View(get(FILTER_Keyword, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo));
            }
        }

        // POST: PettyCashRecords
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, int? FILTER_Approved, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            setViewBag(FILTER_Keyword, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            return View(get(FILTER_Keyword, FILTER_Approved, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PettyCashRecords/Create
        public ActionResult Create(string id)
        {
            if (!UserAccountsController.getUserAccess(Session).PettyCashRecords_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            return View();
        }

        //POST: PettyCashRecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string id, string JsonPettyCashRecords)
        {
            List<SaleInvoicesModel> saleinvoices = SaleInvoicesController.get(Session, id).OrderBy(x=>x.Timestamp).ToList();

            if (ModelState.IsValid)
            {

            }

            return View();
        }

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, int? FILTER_Approved, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_Approved = FILTER_Approved;
            ViewBag.FILTER_chkDateFrom = FILTER_chkDateFrom;
            ViewBag.FILTER_DateFrom = FILTER_DateFrom;
            ViewBag.FILTER_chkDateTo = FILTER_chkDateTo;
            ViewBag.FILTER_DateTo = FILTER_DateTo;
        }

        public JsonResult Update_IsChecked(Guid id, bool value)
        {
            update_IsChecked(id, value);
            return Json(new { Message = "" });
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public List<PettyCashRecordsModel> get(string FILTER_Keyword, int? FILTER_Approved,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo) 
        { return get(Session, null, FILTER_Keyword, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo, FILTER_Approved); }
        public PettyCashRecordsModel get(Guid Id) { return get(Session, Id, null, false, null, false, null, null).FirstOrDefault(); }
        public static List<PettyCashRecordsModel> get(HttpSessionStateBase Session, Guid? Id, string FILTER_Keyword, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo, 
            int? IsChecked)
        {
            Guid Branches_Id = Helper.getActiveBranchId(Session);

            if (FILTER_chkDateFrom == null || !(bool)FILTER_chkDateFrom)
                FILTER_DateFrom = null;

            if (FILTER_chkDateTo == null || !(bool)FILTER_chkDateTo)
                FILTER_DateTo = null;

            return new DBContext().Database.SqlQuery<PettyCashRecordsModel>(@"
                    SELECT PettyCashRecords.*,
                        PettyCashRecordsCategories.Name AS PettyCashRecordsCategories_Name,
                        LEFT(UserAccounts.Fullname, 
                                CASE 
                                    WHEN charindex(' ', UserAccounts.Fullname) = 0 
                                    THEN LEN(UserAccounts.Fullname) 
                                    ELSE charindex(' ', UserAccounts.Fullname) - 1 
                                END
                            ) AS UserAccounts_Firstname,
                        InitialBalance.Amount + (SUM(PettyCashRecords.Amount) OVER(ORDER BY PettyCashRecords.Timestamp ASC)) AS Balance
                    FROM PettyCashRecords
                        LEFT JOIN PettyCashRecordsCategories ON PettyCashRecordsCategories.Id = PettyCashRecords.PettyCashRecordsCategories_Id
                        LEFT JOIN UserAccounts ON UserAccounts.Id = PettyCashRecords.UserAccounts_Id_TEMP
                        LEFT JOIN (
                                SELECT 1 AS Id, SUM(PettyCashRecords.Amount) AS Amount
                                FROM PettyCashRecords
                                WHERE 1=1
                                    AND (@FILTER_DateFrom IS NULL OR PettyCashRecords.Timestamp < @FILTER_DateFrom)
                            ) InitialBalance ON InitialBalance.Id = 1
                    WHERE 1=1
						AND (@Id IS NULL OR PettyCashRecords.Id = @Id)
						AND (@Id IS NOT NULL OR (
    						(@FILTER_Keyword IS NULL OR (PettyCashRecords.No LIKE '%'+@FILTER_Keyword+'%'))
                            AND (@FILTER_DateFrom IS NULL OR PettyCashRecords.Timestamp >= @FILTER_DateFrom)
                            AND (@FILTER_DateTo IS NULL OR PettyCashRecords.Timestamp <= @FILTER_DateTo)
                            AND (@IsChecked IS NULL OR PettyCashRecords.IsChecked = @IsChecked)
                        ))
					ORDER BY PettyCashRecords.Timestamp ASC
                ",
                DBConnection.getSqlParameter(PettyCashRecordsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                DBConnection.getSqlParameter("FILTER_DateFrom", FILTER_DateFrom),
                DBConnection.getSqlParameter("FILTER_DateTo", Util.getAsEndDate(FILTER_DateTo)),
                DBConnection.getSqlParameter(PettyCashRecordsModel.COL_IsChecked.Name, IsChecked)
            ).ToList();
        }

        public static void add(DBContext db, PettyCashRecordsModel model)
        {                    
            db.Database.ExecuteSqlCommand(@"

	                -- INCREMENT LAST HEX NUMBER
	                DECLARE @HexLength int = 5, @LastHex_String varchar(5), @NewNo varchar(5)
	                SELECT @LastHex_String = ISNULL(MAX(No),'') From PettyCashRecords	
	                DECLARE @LastHex_Int int
	                SELECT @LastHex_Int = CONVERT(INT, CONVERT(VARBINARY, REPLICATE('0', LEN(@LastHex_String)%2) + @LastHex_String, 2)) --@LastHex_String length must be even number of digits to convert to int
	                SET @NewNo = RIGHT(CONVERT(NVARCHAR(10), CONVERT(VARBINARY(8), @LastHex_Int + 1), 1),@HexLength)

                    INSERT INTO PettyCashRecords   (Id, Branches_Id, RefId, No,    Timestamp, PettyCashRecordsCategories_Id, Notes, Amount, IsChecked, UserAccounts_Id_TEMP, ExpenseCategories_Id) 
                                            VALUES(@Id,@Branches_Id,@RefId,@NewNo,@Timestamp,@PettyCashRecordsCategories_Id,@Notes,@Amount,@IsChecked,@UserAccounts_Id_TEMP,@ExpenseCategories_Id);
                ",
                DBConnection.getSqlParameter(PettyCashRecordsModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(PettyCashRecordsModel.COL_Branches_Id.Name, model.Branches_Id),
                DBConnection.getSqlParameter(PettyCashRecordsModel.COL_RefId.Name, model.RefId),
                DBConnection.getSqlParameter(PettyCashRecordsModel.COL_Timestamp.Name, model.Timestamp),
                DBConnection.getSqlParameter(PettyCashRecordsModel.COL_PettyCashRecordsCategories_Id.Name, model.PettyCashRecordsCategories_Id),
                DBConnection.getSqlParameter(PettyCashRecordsModel.COL_Notes.Name, model.Notes),
                DBConnection.getSqlParameter(PettyCashRecordsModel.COL_Amount.Name, model.Amount),
                DBConnection.getSqlParameter(PettyCashRecordsModel.COL_IsChecked.Name, model.IsChecked),
                DBConnection.getSqlParameter(PettyCashRecordsModel.COL_UserAccounts_Id_TEMP.Name, model.UserAccounts_Id_TEMP),
                DBConnection.getSqlParameter(PettyCashRecordsModel.COL_ExpenseCategories_Id.Name, model.ExpenseCategories_Id)
            );
        }

        public void update_IsChecked(Guid Id, bool value)
        {
            db.Database.ExecuteSqlCommand(@"
                UPDATE PettyCashRecords 
                SET
                    IsChecked = @IsChecked
                WHERE PettyCashRecords.Id = @Id;                
            ",
                DBConnection.getSqlParameter(PettyCashRecordsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(PettyCashRecordsModel.COL_IsChecked.Name, value)
            );

            ActivityLogsController.AddEditLog(db, Session, Id, string.Format(PettyCashRecordsModel.COL_IsChecked.LogDisplay, null, value));
            db.SaveChanges();
        }

        /******************************************************************************************************************************************************/
    }
}