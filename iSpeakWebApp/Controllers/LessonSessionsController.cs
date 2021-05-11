﻿using System;
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
                List<HourlyRatesModel> hourlyRates = db.HourlyRates.Where(x => x.UserAccounts_Id_TEMP == model.Tutor_UserAccounts_Id_TEMP).ToList();
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

                    db.PayrollPaymentItems.Add(new PayrollPaymentItemsModel()
                    {
                        Id = PayrollPaymentItems_Id,
                        Timestamp = model.Timestamp,
                        Description = null,
                        Hour = model.IsWaiveTutorFee ? 0 : model.SessionHours,
                        UserAccounts_Id_TEMP = model.Tutor_UserAccounts_Id_TEMP,
                        Branches_Id = model.Branches_Id,
                        TutorTravelCost = model.TutorTravelCost,
                        HourlyRate = HourlyRate,
                        Amount = model.IsWaiveTutorFee ? 0 : (model.SessionHours * HourlyRate) + model.TutorTravelCost
                    });
                }

                db.SaveChanges();

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

        public JsonResult UpdateDeleted(Guid id, bool value, string CancelNotes)
        {
            update_Deleted(id, value, CancelNotes);
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

            return new DBContext().Database.SqlQuery<LessonSessionsModel>(@"
                    SELECT LessonSessions.*,
                        SaleInvoices.No AS SaleInvoices_No,
                        SaleInvoiceItems.Description AS SaleInvoiceItems_Description,
                        Student_UserAccounts.Fullname AS Student_UserAccounts_Fullname,
                        Tutor_UserAccounts.Fullname AS Tutor_UserAccounts_Fullname,
                        ROW_NUMBER() OVER (ORDER BY LessonSessions.Timestamp DESC) AS InitialRowNumber
                    FROM LessonSessions
                        LEFT JOIN SaleInvoiceItems ON SaleInvoiceItems.Id = LessonSessions.SaleInvoiceItems_Id
                        LEFT JOIN SaleInvoices ON SaleInvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                        LEFT JOIN UserAccounts Student_UserAccounts ON Student_UserAccounts.Id = SaleInvoices.Customer_UserAccounts_Id
                        LEFT JOIN UserAccounts Tutor_UserAccounts ON Tutor_UserAccounts.Id = LessonSessions.Tutor_UserAccounts_Id_TEMP
                    WHERE 1=1
						AND (@Id IS NULL OR LessonSessions.Id = @Id)
						AND (@Id IS NOT NULL OR (
    						LessonSessions.Branches_Id = @Branches_Id
                            AND (@FILTER_Keyword IS NULL OR (SaleInvoices.No LIKE '%'+@FILTER_Keyword+'%'))
                            AND (@FILTER_DateFrom IS NULL OR LessonSessions.Timestamp >= @FILTER_DateFrom)
                            AND (@FILTER_DateTo IS NULL OR LessonSessions.Timestamp <= @FILTER_DateTo)
                            AND (@Deleted IS NULL OR LessonSessions.Deleted = @Deleted)
    						AND (@FILTER_InvoiceNo IS NULL OR (LessonSessions.SaleInvoiceItems_Id IN (                                
                                SELECT SaleInvoiceItems.Id
                                FROM SaleInvoiceItems 
	                                LEFT JOIN SaleInvoices ON Saleinvoices.Id = SaleInvoiceItems.SaleInvoices_Id
                                WHERE SaleInvoices.No = @FILTER_InvoiceNo
                            )))
                        ))
					ORDER BY LessonSessions.Timestamp DESC
                ",
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
                INSERT INTO LessonSessions   (Id, Branches_Id, Timestamp, SaleInvoiceItems_Id, SessionHours, Review, InternalNotes, Deleted, Tutor_UserAccounts_Id_TEMP, HourlyRates_Rate, TravelCost, TutorTravelCost, Adjustment, PayrollPaymentItems_Id, Notes_Cancel, IsScheduleChange, IsWaiveTutorFee) 
                                      VALUES(@Id,@Branches_Id,@Timestamp,@SaleInvoiceItems_Id,@SessionHours,@Review,@InternalNotes,@Deleted,@Tutor_UserAccounts_Id_TEMP,@HourlyRates_Rate,@TravelCost,@TutorTravelCost,@Adjustment,@PayrollPaymentItems_Id,@Notes_Cancel,@IsScheduleChange,@IsWaiveTutorFee);
            ",
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Id.Name, model.Id),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Branches_Id.Name, model.Branches_Id),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Timestamp.Name, model.Timestamp),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_SaleInvoiceItems_Id.Name, model.SaleInvoiceItems_Id),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_SessionHours.Name, model.SessionHours),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Review.Name, model.Review),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_InternalNotes.Name, model.InternalNotes),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Deleted.Name, model.Deleted),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Tutor_UserAccounts_Id_TEMP.Name, model.Tutor_UserAccounts_Id_TEMP),
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

        public void update_Deleted(Guid Id, bool value, string CancelNotes)
        {
            db.Database.ExecuteSqlCommand(@"
                UPDATE LessonSessions 
                SET
                    Deleted = @Deleted
                WHERE LessonSessions.Id = @Id;                
            ",
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Id.Name, Id),
                DBConnection.getSqlParameter(LessonSessionsModel.COL_Deleted.Name, value)
            );

            ActivityLogsController.AddEditLog(db, Session, Id, string.Format(LessonSessionsModel.COL_Deleted.LogDisplay, null, value));
            db.SaveChanges();
        }

        /******************************************************************************************************************************************************/
    }
}