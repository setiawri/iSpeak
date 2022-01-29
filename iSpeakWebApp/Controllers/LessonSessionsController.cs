using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using iSpeakWebApp.Models;
using Newtonsoft.Json;
using LIBUtil;
using LIBWebMVC;

namespace iSpeakWebApp.Controllers
{
    public class LessonSessionsController : Controller
    {
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: LessonSessions
        public ActionResult Index(int? rss, string FILTER_Keyword, string FILTER_InvoiceNo, int? FILTER_Cancelled, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (!UserAccountsController.getUserAccess(Session).LessonSessions_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (UtilWebMVC.hasNoFilter(FILTER_Keyword, FILTER_InvoiceNo, FILTER_Cancelled, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo))
            {
                FILTER_chkDateFrom = true;
                FILTER_DateFrom = Helper.getCurrentDateTime();
            }

            setViewBag(FILTER_Keyword, FILTER_InvoiceNo, FILTER_Cancelled, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                List<LessonSessionsModel> models = get(FILTER_Keyword, FILTER_InvoiceNo, FILTER_Cancelled, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo)
                    .OrderByDescending(x => x.Timestamp).ToList();
                return View(models);
            }
        }

        // POST: LessonSessions
        [HttpPost]
        public ActionResult Index(string FILTER_Keyword, string FILTER_InvoiceNo, int? FILTER_Cancelled, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            setViewBag(FILTER_Keyword, FILTER_InvoiceNo, FILTER_Cancelled, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            List<LessonSessionsModel> models = get(FILTER_Keyword, FILTER_InvoiceNo, FILTER_Cancelled, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo)
                .OrderByDescending(x => x.Timestamp).ToList();
            return View(models);
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: LessonSessions/Create
        public ActionResult Create(string FILTER_Keyword, string FILTER_InvoiceNo, int? FILTER_Cancelled,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (!UserAccountsController.getUserAccess(Session).LessonSessions_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(FILTER_Keyword, FILTER_InvoiceNo, FILTER_Cancelled, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            return View();
        }

        //POST: LessonSessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string JsonLessonSessions, string FILTER_Keyword, string FILTER_InvoiceNo, int? FILTER_Cancelled,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            List<LessonSessionsModel> LessonSessions = new List<LessonSessionsModel>();
            if (string.IsNullOrEmpty(JsonLessonSessions))
                return returnView(LessonSessions, "Please add at least one student");

            LessonSessions = JsonConvert.DeserializeObject<List<LessonSessionsModel>>(JsonLessonSessions);
            LessonSessionsModel model = LessonSessions[0];
            model.Branches_Id = Helper.getActiveBranchId(Session);

            if (ModelState.IsValid)
            {
                Guid PayrollPaymentItems_Id = Guid.NewGuid();

                //verify remaining hours is enough to cover the session hours. show error for the first error only
                string SaleInvoiceItems_IdList = string.Join(",", LessonSessions.Select(x => x.SaleInvoiceItems_Id.ToString()).ToArray());
                List<SaleInvoiceItemsModel> SaleInvoiceItems = SaleInvoiceItemsController.get_by_IdList(SaleInvoiceItems_IdList);
                List<SaleInvoiceItemsModel> insufficientRemainingHours = SaleInvoiceItems.Where(x => x.SessionHours_Remaining < model.SessionHours).ToList();
                if (insufficientRemainingHours.Count > 0)
                    return returnView(LessonSessions, string.Format("Insufficient remaining hours for student {0}", insufficientRemainingHours[0].Customer_UserAccounts_Name));

                //set tutor pay rate
                List<HourlyRatesModel> hourlyRates = HourlyRatesController.get(null, null, model.Tutor_UserAccounts_Id);
                bool isFullTimeTutor = false;
                foreach (HourlyRatesModel hourlyRate in hourlyRates)
                {
                    if (hourlyRate.FullTimeTutorPayrate > 0)
                    {
                        isFullTimeTutor = true;
                        continue;
                    }
                }

                foreach (LessonSessionsModel session in LessonSessions)
                {
                    SaleInvoiceItemsModel saleInvoiceItem = SaleInvoiceItems.Where(x => x.Id == session.SaleInvoiceItems_Id).FirstOrDefault();

                    session.Id = Guid.NewGuid();
                    session.Branches_Id = model.Branches_Id;
                    session.HourlyRates_Rate = 0;
                    session.SessionHours = session.IsScheduleChange ? 0 : session.SessionHours;
                    session.TravelCost = session.IsScheduleChange ? 0 : (int)Math.Ceiling((saleInvoiceItem.TravelCost / saleInvoiceItem.SessionHours) * session.SessionHours);
                    session.TutorTravelCost = session.IsScheduleChange ? 0 : (int)Math.Ceiling((saleInvoiceItem.TutorTravelCost / saleInvoiceItem.SessionHours) * session.SessionHours);
                    session.PayrollPaymentItems_Id = session.IsScheduleChange ? (Guid?)null : PayrollPaymentItems_Id;

                    //Calculate tutor payrate
                    if(!isFullTimeTutor && hourlyRates.Count > 0)
                    {
                        foreach (HourlyRatesModel hourlyRate in hourlyRates)
                        {
                            session.HourlyRates_Rate = Math.Ceiling(hourlyRate.Rate / LessonSessions.Count);
                            if (hourlyRate.LessonPackages_Id == saleInvoiceItem.LessonPackages_Id) //rate for the exact lesson package
                                break;
                        }
                    }
                    model.TutorTravelCost = session.TutorTravelCost;

                    add(session);

                    //adjust remaining session hours
                    saleInvoiceItem.SessionHours_Remaining -= session.SessionHours;
                    SaleInvoiceItemsController.update_SessionHours_Remaining(db, Session, saleInvoiceItem.Id, saleInvoiceItem.SessionHours_Remaining,
                        string.Format("Lesson Session on {0:dd/MM/yy HH:mm} for {1:N2} hours. Remaining hours: {2:N2} hours.", session.Timestamp, session.SessionHours, saleInvoiceItem.SessionHours_Remaining));
                }

                //create payrollpaymentitem
                if (!model.IsScheduleChange)
                {
                    //Calculate tutor payrate
                    decimal HourlyRate = 0;
                    if (!isFullTimeTutor && hourlyRates.Count > 0)
                    {
                        foreach (HourlyRatesModel hourlyRate in hourlyRates)
                        {
                            HourlyRate = hourlyRate.Rate;
                            if (hourlyRate.Branches_Id == model.Branches_Id) //rate for the exact lesson package
                                break;
                        }
                    }

                    PayrollPaymentItemsController.add(new PayrollPaymentItemsModel()
                    {
                        Id = PayrollPaymentItems_Id,
                        PayrollPayments_Id = null,
                        Timestamp = model.Timestamp,
                        Description = null,
                        Hour = model.IsWaiveTutorFee ? 0 : model.SessionHours,
                        HourlyRate = HourlyRate,
                        TutorTravelCost = model.TutorTravelCost,
                        Amount = PayrollPaymentItemsController.calculateAmount(model.IsWaiveTutorFee, model.SessionHours, HourlyRate, model.TutorTravelCost),
                        UserAccounts_Id = model.Tutor_UserAccounts_Id,
                        Branches_Id = model.Branches_Id,
                        IsFullTime = false
                    });
                }

                return RedirectToAction(nameof(Index), new
                {
                    FILTER_Keyword = FILTER_Keyword,
                    FILTER_InvoiceNo = FILTER_InvoiceNo,
                    FILTER_Cancelled = FILTER_Cancelled,
                    FILTER_chkDateFrom = FILTER_chkDateFrom,
                    FILTER_DateFrom = FILTER_DateFrom,
                    FILTER_chkDateTo = FILTER_chkDateTo,
                    FILTER_DateTo = FILTER_DateTo
                });
            }

            return returnView(LessonSessions, null);
        }

        public ActionResult returnView(List<LessonSessionsModel> LessonSessions, string errorMessage)
        {
            if(!string.IsNullOrEmpty(errorMessage))
                UtilWebMVC.setBootboxMessage(this, errorMessage);

            ViewData["LessonSessions"] = LessonSessions;
            return View();
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: LessonSessions/Edit/{id}
        public ActionResult Edit(Guid? id, string FILTER_Keyword, string FILTER_InvoiceNo, int? FILTER_Cancelled,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (!UserAccountsController.getUserAccess(Session).LessonSessions_Edit && !UserAccountsController.getUserAccess(Session).LessonSessions_EditReviewAndInternalNotes)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(FILTER_Keyword, FILTER_InvoiceNo, FILTER_Cancelled, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            return View(get(id.Value));
        }

        // POST: LessonSessions/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LessonSessionsModel modifiedModel, string FILTER_Keyword, string FILTER_InvoiceNo, int? FILTER_Cancelled,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            if (ModelState.IsValid)
            {
                LessonSessionsModel originalModel = get(modifiedModel.Id);

                //without the specified access, some fields are excluded in edit form resulting in no value. Copy values from original model
                if (!UserAccountsController.getUserAccess(Session).LessonSessions_Edit)
                {
                    modifiedModel.HourlyRates_Rate = originalModel.HourlyRates_Rate;
                    modifiedModel.TravelCost = originalModel.TravelCost;
                    modifiedModel.TutorTravelCost = originalModel.TutorTravelCost;
                }

                string log = string.Empty;
                log = Helper.append(log, originalModel.HourlyRates_Rate, modifiedModel.HourlyRates_Rate, LessonSessionsModel.COL_HourlyRates_Rate.LogDisplay);
                log = Helper.append(log, originalModel.TravelCost, modifiedModel.TravelCost, LessonSessionsModel.COL_TravelCost.LogDisplay);
                log = Helper.append(log, originalModel.TutorTravelCost, modifiedModel.TutorTravelCost, LessonSessionsModel.COL_TutorTravelCost.LogDisplay);
                log = Helper.append(log, originalModel.Review, modifiedModel.Review, LessonSessionsModel.COL_Review.LogDisplay);
                log = Helper.append(log, originalModel.InternalNotes, modifiedModel.InternalNotes, LessonSessionsModel.COL_InternalNotes.LogDisplay);

                if (!string.IsNullOrEmpty(log))
                {
                    update(modifiedModel, log);

                    //update payrollitem if rate or travel cost is changed
                    //Tutor Travel Cost is not currently checked against total travel cost amount paid by customer. This edit may cause cost to exceed amount paid by customer.
                    PayrollPaymentItemsModel payrollPaymentItem = PayrollPaymentItemsController.get(Session, originalModel.PayrollPaymentItems_Id);

                    //this is necessary for payrollpaymentitems that has multiple lessonsessions (class)
                    payrollPaymentItem.HourlyRate += (modifiedModel.HourlyRates_Rate - originalModel.HourlyRates_Rate); 
                    payrollPaymentItem.TutorTravelCost += (modifiedModel.TutorTravelCost - originalModel.TutorTravelCost);

                    payrollPaymentItem.Amount = PayrollPaymentItemsController.calculateAmount(originalModel.IsWaiveTutorFee, payrollPaymentItem.Hour, payrollPaymentItem.HourlyRate, modifiedModel.TutorTravelCost);
                    PayrollPaymentItemsController.update(db, Session, payrollPaymentItem);
                }

                return RedirectToAction(nameof(Index), new
                {
                    FILTER_Keyword = FILTER_Keyword,
                    FILTER_InvoiceNo = FILTER_InvoiceNo,
                    FILTER_Cancelled = FILTER_Cancelled,
                    FILTER_chkDateFrom = FILTER_chkDateFrom,
                    FILTER_DateFrom = FILTER_DateFrom,
                    FILTER_chkDateTo = FILTER_chkDateTo,
                    FILTER_DateTo = FILTER_DateTo
                });
            }

            setViewBag(FILTER_Keyword, FILTER_InvoiceNo, FILTER_Cancelled, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo);
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public void setViewBag(string FILTER_Keyword, string FILTER_InvoiceNo, int? FILTER_Cancelled, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo)
        {
            ViewBag.FILTER_Keyword = FILTER_Keyword;
            ViewBag.FILTER_InvoiceNo = FILTER_InvoiceNo;
            ViewBag.FILTER_Cancelled = FILTER_Cancelled;
            ViewBag.FILTER_chkDateFrom = FILTER_chkDateFrom;
            ViewBag.FILTER_DateFrom = FILTER_DateFrom;
            ViewBag.FILTER_chkDateTo = FILTER_chkDateTo;
            ViewBag.FILTER_DateTo = FILTER_DateTo;
        }

        public JsonResult Ajax_Update_Deleted(Guid id, string notes)
        {
            update_Deleted(id, notes);
            return Json(new { Message = "" });
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public List<LessonSessionsModel> get(string FILTER_Keyword, string FILTER_InvoiceNo, int? FILTER_Cancelled,
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo) 
        { return get(Session, null, FILTER_Keyword, FILTER_InvoiceNo, FILTER_chkDateFrom, FILTER_DateFrom, FILTER_chkDateTo, FILTER_DateTo, FILTER_Cancelled); }
        public LessonSessionsModel get(Guid Id) { return get(Session, Id, null, null, false, null, false, null, null).FirstOrDefault(); }
        public static List<LessonSessionsModel> get(HttpSessionStateBase Session, Guid? Id, string FILTER_Keyword, string FILTER_InvoiceNo, 
            bool? FILTER_chkDateFrom, DateTime? FILTER_DateFrom, bool? FILTER_chkDateTo, DateTime? FILTER_DateTo, 
            int? Cancelled)
        {
            Guid Branches_Id = Helper.getActiveBranchId(Session);

            if (FILTER_chkDateFrom == null || !(bool)FILTER_chkDateFrom)
                FILTER_DateFrom = null;

            if (FILTER_chkDateTo == null || !(bool)FILTER_chkDateTo)
                FILTER_DateTo = null;

            string ShowOnlyOwnUserDataClause = "";
            if (UserAccountsController.getShowOnlyUserData(Session))
                ShowOnlyOwnUserDataClause = string.Format(" AND (Student_UserAccounts.Id = '{0}' OR Tutor_UserAccounts.Id = '{0}')", UserAccountsController.getUserId(Session));

            string sql = string.Format(@"
                    SELECT LessonSessions.*,
                        SaleInvoices.No AS SaleInvoices_No,
                        SaleInvoiceItems.Description AS SaleInvoiceItems_Description,
                        Student_UserAccounts.Fullname AS Student_UserAccounts_Fullname,
                        Student_UserAccounts.No AS Student_UserAccounts_No,
                        Tutor_UserAccounts.Fullname AS Tutor_UserAccounts_Fullname,
                        Tutor_UserAccounts.No AS Tutor_UserAccounts_No,
                        ROW_NUMBER() OVER (ORDER BY LessonSessions.Timestamp DESC) AS InitialRowNumber
                    FROM LessonSessions
                        LEFT JOIN SaleInvoiceItems ON SaleInvoiceItems.Id = LessonSessions.SaleInvoiceItems_Id
                        LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                        LEFT JOIN UserAccounts Student_UserAccounts ON Student_UserAccounts.Id = SaleInvoices.Customer_UserAccounts_Id
                        LEFT JOIN UserAccounts Tutor_UserAccounts ON Tutor_UserAccounts.Id = LessonSessions.Tutor_UserAccounts_Id
                    WHERE 1=1
						AND (@Id IS NULL OR LessonSessions.Id = @Id)
						AND (@Id IS NOT NULL OR (
    						LessonSessions.Branches_Id = @Branches_Id
                            AND (@FILTER_Keyword IS NULL OR (
                                    LessonSessions.No LIKE '%'+@FILTER_Keyword+'%'
                                ))
                            AND (@FILTER_DateFrom IS NULL OR LessonSessions.Timestamp >= @FILTER_DateFrom)
                            AND (@FILTER_DateTo IS NULL OR LessonSessions.Timestamp <= @FILTER_DateTo)
                            AND (@Deleted IS NULL OR LessonSessions.Deleted = @Deleted)
    						AND (@FILTER_InvoiceNo IS NULL OR (LessonSessions.SaleInvoiceItems_Id IN (                                
                                SELECT SaleInvoiceItems.Id
                                FROM SaleInvoiceItems 
	                                LEFT JOIN SaleInvoices ON Saleinvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                                WHERE SaleInvoices.No = @FILTER_InvoiceNo
                            )))
                            {0}
                        ))
					ORDER BY LessonSessions.Timestamp DESC
                ", ShowOnlyOwnUserDataClause);

            return new DBContext().Database.SqlQuery<LessonSessionsModel>(sql,
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Branches_Id.Name, Branches_Id),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Deleted.Name, Cancelled),
                DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword),
                DBConnection.getSqlParameter("FILTER_InvoiceNo", FILTER_InvoiceNo),
                DBConnection.getSqlParameter("FILTER_DateFrom", FILTER_DateFrom),
                DBConnection.getSqlParameter("FILTER_DateTo", Util.getAsEndDate(FILTER_DateTo))
            ).ToList();
        }

        public void add(LessonSessionsModel model)
        {
            db.Database.ExecuteSqlCommand(@"       

	            -- INCREMENT LAST HEX NUMBER
	            DECLARE @HexLength int = 5, @LastHex_String varchar(5), @NewNo varchar(5)
	            SELECT @LastHex_String = ISNULL(MAX(No),'') From LessonSessions	
	            DECLARE @LastHex_Int int
	            SELECT @LastHex_Int = CONVERT(INT, CONVERT(VARBINARY, REPLICATE('0', LEN(@LastHex_String)%2) + @LastHex_String, 2)) --@LastHex_String length must be even number of digits to convert to int
	            SET @NewNo = RIGHT(CONVERT(NVARCHAR(10), CONVERT(VARBINARY(8), @LastHex_Int + 1), 1),@HexLength)

                INSERT INTO LessonSessions   (Id, No,    Branches_Id, Timestamp, SaleInvoiceItems_Id, SessionHours, Review, InternalNotes, Deleted, Tutor_UserAccounts_Id, HourlyRates_Rate, TravelCost, TutorTravelCost, Adjustment, PayrollPaymentItems_Id, Notes_Cancel, IsScheduleChange, IsWaiveTutorFee) 
                                      VALUES(@Id,@NewNo,@Branches_Id,@Timestamp,@SaleInvoiceItems_Id,@SessionHours,@Review,@InternalNotes,@Deleted,@Tutor_UserAccounts_Id,@HourlyRates_Rate,@TravelCost,@TutorTravelCost,@Adjustment,@PayrollPaymentItems_Id,@Notes_Cancel,@IsScheduleChange,@IsWaiveTutorFee);
            ",
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Branches_Id.Name, model.Branches_Id),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Timestamp.Name, model.Timestamp),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_SaleInvoiceItems_Id.Name, model.SaleInvoiceItems_Id),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_SessionHours.Name, model.SessionHours),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Review.Name, model.Review),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_InternalNotes.Name, model.InternalNotes),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Deleted.Name, model.Deleted),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Tutor_UserAccounts_Id.Name, model.Tutor_UserAccounts_Id),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_HourlyRates_Rate.Name, model.HourlyRates_Rate),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_TravelCost.Name, model.TravelCost),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_TutorTravelCost.Name, model.TutorTravelCost),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Adjustment.Name, model.Adjustment),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_PayrollPaymentItems_Id.Name, model.PayrollPaymentItems_Id),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Notes_Cancel.Name, model.Notes_Cancel),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_IsScheduleChange.Name, model.IsScheduleChange),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_IsWaiveTutorFee.Name, model.IsWaiveTutorFee)
            );

            ActivityLogsController.AddCreateLog(db, Session, model.Id);
        }

        public void update(LessonSessionsModel model, string log)
        {
            WebDBConnection.Update(db.Database, "LessonSessions",
                    DBConnection.getSqlParameter(LessonSessionsModel.COL_Id.Name, model.Id),
                    DBConnection.getSqlParameter(LessonSessionsModel.COL_HourlyRates_Rate.Name, model.HourlyRates_Rate),
                    DBConnection.getSqlParameter(LessonSessionsModel.COL_TravelCost.Name, model.TravelCost),
                    DBConnection.getSqlParameter(LessonSessionsModel.COL_TutorTravelCost.Name, model.TutorTravelCost),
                    DBConnection.getSqlParameter(LessonSessionsModel.COL_Review.Name, model.Review),
                    DBConnection.getSqlParameter(LessonSessionsModel.COL_InternalNotes.Name, model.InternalNotes)
                );
            ActivityLogsController.AddEditLog(db, Session, model.Id, log);
            db.SaveChanges();
        }

        public void update_Deleted(Guid Id, string CancelNotes)
        {
            db.Database.ExecuteSqlCommand(@"
                DELETE PayrollPaymentItems WHERE PayrollPaymentItems.Id = (SELECT LessonSessions.PayrollPaymentItems_Id FROM LessonSessions WHERE LessonSessions.Id = @Id)

                UPDATE LessonSessions 
                SET
                    Deleted = @Deleted,
                    Notes_Cancel = @Notes_Cancel,
                    PayrollPaymentItems_Id = NULL
                WHERE LessonSessions.Id = @Id;           

                UPDATE SaleInvoiceItems 
                SET SessionHours_Remaining = SessionHours_Remaining + (SELECT LessonSessions.SessionHours FROM LessonSessions WHERE LessonSessions.Id = @Id)  
                WHERE SaleInvoiceItems.Id = (
                        SELECT LessonSessions.SaleInvoiceItems_Id 
                        FROM LessonSessions 
                        WHERE LessonSessions.Id = @Id
                    );    
            ",
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Deleted.Name, 1),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Notes_Cancel.Name, CancelNotes)
            );

            ActivityLogsController.AddEditLog(db, Session, Id, string.Format(LessonSessionsModel.COL_Deleted.LogDisplay, null, true));
            db.SaveChanges();
        }

        /******************************************************************************************************************************************************/
    }
}