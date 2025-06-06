﻿using iSpeakWebApp.Models;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Web.Mvc;
using LIBUtil;
using System.Reflection;
using System.Web;

/*
 * To add new user access:
 * - add items in UserAccountRolesModel
 * - add items to database table UserAccountRoles
 * - add items in UserAccountRoles > Edit.cshtml
 * - add items in Controller Post UserAccountRolesController.Edit() 
 * - add items in Controller UserAccountRolesController.getAccesses() 
 * - update views that use the items including main layout file
 */

namespace iSpeakWebApp.Controllers
{
    /*
     * User Account Roles is GLOBAL and are NOT filtered by Franchise. Changes must be made by central.
     */

    public class UserAccountRolesController : Controller
    {
        public const string NAME = "UserAccountRoles";

        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: UserAccountRoles
        public ActionResult Index(int? rss)
        {
            if (!UserAccountsController.getUserAccess(Session).UserAccountRoles_View)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            ViewBag.RemoveDatatablesStateSave = rss;

            return View(get(this.Session));
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: UserAccountRoles/Create
        public ActionResult Create()
        {
            if (!UserAccountsController.getUserAccess(Session).UserAccountRoles_Add)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            setViewBag(this);
            return View(new UserAccountRolesModel());
        }

        // POST: UserAccountRoles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserAccountRolesModel model)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Create, null, model.Name))
                    ModelState.AddModelError(UserAccountRolesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    db.UserAccountRoles.Add(model);
                    db.SaveChanges();
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    return RedirectToAction(nameof(Edit), new { id = model.Id });
                }
            }

            setViewBag(this);
            return View(model);
        }


        /* EDIT ***********************************************************************************************************************************************/

        // GET: UserAccountRoles/Edit/{id}
        public ActionResult Edit(Guid? id)
        {
            if (!UserAccountsController.getUserAccess(Session).UserAccountRoles_Edit)
                return RedirectToAction(nameof(HomeController.Index), "Home");

            if (id == null)
                return RedirectToAction(nameof(Index));

            setViewBag(this);
            var model = get(id);
            return View(model);
        }

        // POST: UserAccountRoles/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserAccountRolesModel model)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Edit, model.Id, model.Name))
                    ModelState.AddModelError(UserAccountRolesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    UserAccountRolesModel originalModel = get(model.Id);

                    string log = string.Empty;
                    log = Helper.append(log, originalModel.Name, model.Name, UserAccountRolesModel.COL_Name.LogDisplay);

                    log = Helper.addLogForList<UserAccountRolesModel>(log, originalModel.Roles_List, model.Roles_List, UserAccountsModel.COL_Roles.LogDisplay);

                    //ActivityLogs
                    log = Helper.append(log, originalModel.ActivityLogs_Notes, model.ActivityLogs_Notes, UserAccountRolesModel.COL_ActivityLogs_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.ActivityLogs_View, model.ActivityLogs_View, UserAccountRolesModel.COL_ActivityLogs_View.LogDisplay);

                    //Reminders
                    log = Helper.append(log, originalModel.Reminders_Notes, model.Reminders_Notes, UserAccountRolesModel.COL_Reminders_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Reminders_Add, model.Reminders_Add, UserAccountRolesModel.COL_Reminders_Add.LogDisplay);
                    log = Helper.append(log, originalModel.Reminders_Edit, model.Reminders_Edit, UserAccountRolesModel.COL_Reminders_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.Reminders_View, model.Reminders_View, UserAccountRolesModel.COL_Reminders_View.LogDisplay);

                    //Birthdays
                    log = Helper.append(log, originalModel.Birthdays_Notes, model.Birthdays_Notes, UserAccountRolesModel.COL_Birthdays_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Birthdays_View, model.Birthdays_View, UserAccountRolesModel.COL_Birthdays_View.LogDisplay);

                    //UserAccounts
                    log = Helper.append(log, originalModel.UserAccounts_Notes, model.UserAccounts_Notes, UserAccountRolesModel.COL_UserAccounts_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.UserAccounts_Add, model.UserAccounts_Add, UserAccountRolesModel.COL_UserAccounts_Add.LogDisplay);
                    log = Helper.append(log, originalModel.UserAccounts_Edit, model.UserAccounts_Edit, UserAccountRolesModel.COL_UserAccounts_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.UserAccounts_View, model.UserAccounts_View, UserAccountRolesModel.COL_UserAccounts_View.LogDisplay);
                    log = Helper.append(log, originalModel.UserAccounts_ResetPassword, model.UserAccounts_ResetPassword, UserAccountRolesModel.COL_UserAccounts_ResetPassword.LogDisplay);
                    log = Helper.append(log, originalModel.UserAccounts_ViewAllRoles, model.UserAccounts_ViewAllRoles, UserAccountRolesModel.COL_UserAccounts_ViewAllRoles.LogDisplay);

                    //User Account Roles
                    log = Helper.append(log, originalModel.UserAccountRoles_Notes, model.UserAccountRoles_Notes, UserAccountRolesModel.COL_UserAccountRoles_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.UserAccountRoles_Add, model.UserAccountRoles_Add, UserAccountRolesModel.COL_UserAccountRoles_Add.LogDisplay);
                    log = Helper.append(log, originalModel.UserAccountRoles_Edit, model.UserAccountRoles_Edit, UserAccountRolesModel.COL_UserAccountRoles_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.UserAccountRoles_View, model.UserAccountRoles_View, UserAccountRolesModel.COL_UserAccountRoles_View.LogDisplay);

                    //Settings
                    log = Helper.append(log, originalModel.Settings_Notes, model.Settings_Notes, UserAccountRolesModel.COL_Settings_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Settings_Edit, model.Settings_Edit, UserAccountRolesModel.COL_Settings_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.Settings_View, model.Settings_View, UserAccountRolesModel.COL_Settings_View.LogDisplay);

                    //Franchises
                    log = Helper.append(log, originalModel.Franchises_Notes, model.Franchises_Notes, UserAccountRolesModel.COL_Franchises_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Franchises_Add, model.Franchises_Add, UserAccountRolesModel.COL_Franchises_Add.LogDisplay);
                    log = Helper.append(log, originalModel.Franchises_Edit, model.Franchises_Edit, UserAccountRolesModel.COL_Franchises_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.Franchises_View, model.Franchises_View, UserAccountRolesModel.COL_Franchises_View.LogDisplay);

                    //Branches
                    log = Helper.append(log, originalModel.Branches_Notes, model.Branches_Notes, UserAccountRolesModel.COL_Branches_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Branches_Add, model.Branches_Add, UserAccountRolesModel.COL_Branches_Add.LogDisplay);
                    log = Helper.append(log, originalModel.Branches_Edit, model.Branches_Edit, UserAccountRolesModel.COL_Branches_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.Branches_View, model.Branches_View, UserAccountRolesModel.COL_Branches_View.LogDisplay);

                    //PromotionEvents
                    log = Helper.append(log, originalModel.PromotionEvents_Notes, model.PromotionEvents_Notes, UserAccountRolesModel.COL_PromotionEvents_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.PromotionEvents_Add, model.PromotionEvents_Add, UserAccountRolesModel.COL_PromotionEvents_Add.LogDisplay);
                    log = Helper.append(log, originalModel.PromotionEvents_Edit, model.PromotionEvents_Edit, UserAccountRolesModel.COL_PromotionEvents_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.PromotionEvents_View, model.PromotionEvents_View, UserAccountRolesModel.COL_PromotionEvents_View.LogDisplay);

                    //PettyCashRecordsCategories
                    log = Helper.append(log, originalModel.PettyCashRecordsCategories_Notes, model.PettyCashRecordsCategories_Notes, UserAccountRolesModel.COL_PettyCashRecordsCategories_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.PettyCashRecordsCategories_Add, model.PettyCashRecordsCategories_Add, UserAccountRolesModel.COL_PettyCashRecordsCategories_Add.LogDisplay);
                    log = Helper.append(log, originalModel.PettyCashRecordsCategories_Edit, model.PettyCashRecordsCategories_Edit, UserAccountRolesModel.COL_PettyCashRecordsCategories_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.PettyCashRecordsCategories_View, model.PettyCashRecordsCategories_View, UserAccountRolesModel.COL_PettyCashRecordsCategories_View.LogDisplay);

                    //Languages
                    log = Helper.append(log, originalModel.Languages_Notes, model.Languages_Notes, UserAccountRolesModel.COL_Languages_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Languages_Add, model.Languages_Add, UserAccountRolesModel.COL_Languages_Add.LogDisplay);
                    log = Helper.append(log, originalModel.Languages_Edit, model.Languages_Edit, UserAccountRolesModel.COL_Languages_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.Languages_View, model.Languages_View, UserAccountRolesModel.COL_Languages_View.LogDisplay);

                    //LessonTypes
                    log = Helper.append(log, originalModel.LessonTypes_Notes, model.LessonTypes_Notes, UserAccountRolesModel.COL_LessonTypes_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.LessonTypes_Add, model.LessonTypes_Add, UserAccountRolesModel.COL_LessonTypes_Add.LogDisplay);
                    log = Helper.append(log, originalModel.LessonTypes_Edit, model.LessonTypes_Edit, UserAccountRolesModel.COL_LessonTypes_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.LessonTypes_View, model.LessonTypes_View, UserAccountRolesModel.COL_LessonTypes_View.LogDisplay);

                    //LessonPackages
                    log = Helper.append(log, originalModel.LessonPackages_Notes, model.LessonPackages_Notes, UserAccountRolesModel.COL_LessonPackages_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.LessonPackages_Add, model.LessonPackages_Add, UserAccountRolesModel.COL_LessonPackages_Add.LogDisplay);
                    log = Helper.append(log, originalModel.LessonPackages_Edit, model.LessonPackages_Edit, UserAccountRolesModel.COL_LessonPackages_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.LessonPackages_View, model.LessonPackages_View, UserAccountRolesModel.COL_LessonPackages_View.LogDisplay);

                    //Consignments
                    log = Helper.append(log, originalModel.Consignments_Notes, model.Consignments_Notes, UserAccountRolesModel.COL_Consignments_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Consignments_Add, model.Consignments_Add, UserAccountRolesModel.COL_Consignments_Add.LogDisplay);
                    log = Helper.append(log, originalModel.Consignments_Edit, model.Consignments_Edit, UserAccountRolesModel.COL_Consignments_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.Consignments_View, model.Consignments_View, UserAccountRolesModel.COL_Consignments_View.LogDisplay);

                    //Vouchers
                    log = Helper.append(log, originalModel.Vouchers_Notes, model.Vouchers_Notes, UserAccountRolesModel.COL_Vouchers_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Vouchers_Add, model.Vouchers_Add, UserAccountRolesModel.COL_Vouchers_Add.LogDisplay);
                    log = Helper.append(log, originalModel.Vouchers_Edit, model.Vouchers_Edit, UserAccountRolesModel.COL_Vouchers_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.Vouchers_View, model.Vouchers_View, UserAccountRolesModel.COL_Vouchers_View.LogDisplay);

                    //Suppliers
                    log = Helper.append(log, originalModel.Suppliers_Notes, model.Suppliers_Notes, UserAccountRolesModel.COL_Suppliers_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Suppliers_Add, model.Suppliers_Add, UserAccountRolesModel.COL_Suppliers_Add.LogDisplay);
                    log = Helper.append(log, originalModel.Suppliers_Edit, model.Suppliers_Edit, UserAccountRolesModel.COL_Suppliers_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.Suppliers_View, model.Suppliers_View, UserAccountRolesModel.COL_Suppliers_View.LogDisplay);

                    //Units
                    log = Helper.append(log, originalModel.Units_Notes, model.Units_Notes, UserAccountRolesModel.COL_Units_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Units_Add, model.Units_Add, UserAccountRolesModel.COL_Units_Add.LogDisplay);
                    log = Helper.append(log, originalModel.Units_Edit, model.Units_Edit, UserAccountRolesModel.COL_Units_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.Units_View, model.Units_View, UserAccountRolesModel.COL_Units_View.LogDisplay);

                    //ExpenseCategories
                    log = Helper.append(log, originalModel.ExpenseCategories_Notes, model.ExpenseCategories_Notes, UserAccountRolesModel.COL_ExpenseCategories_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.ExpenseCategories_Add, model.ExpenseCategories_Add, UserAccountRolesModel.COL_ExpenseCategories_Add.LogDisplay);
                    log = Helper.append(log, originalModel.ExpenseCategories_Edit, model.ExpenseCategories_Edit, UserAccountRolesModel.COL_ExpenseCategories_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.ExpenseCategories_View, model.ExpenseCategories_View, UserAccountRolesModel.COL_ExpenseCategories_View.LogDisplay);

                    //Services
                    log = Helper.append(log, originalModel.Services_Notes, model.Services_Notes, UserAccountRolesModel.COL_Services_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Services_Add, model.Services_Add, UserAccountRolesModel.COL_Services_Add.LogDisplay);
                    log = Helper.append(log, originalModel.Services_Edit, model.Services_Edit, UserAccountRolesModel.COL_Services_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.Services_View, model.Services_View, UserAccountRolesModel.COL_Services_View.LogDisplay);

                    //Products
                    log = Helper.append(log, originalModel.Products_Notes, model.Products_Notes, UserAccountRolesModel.COL_Products_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Products_Add, model.Products_Add, UserAccountRolesModel.COL_Products_Add.LogDisplay);
                    log = Helper.append(log, originalModel.Products_Edit, model.Products_Edit, UserAccountRolesModel.COL_Products_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.Products_View, model.Products_View, UserAccountRolesModel.COL_Products_View.LogDisplay);

                    //SaleInvoices
                    log = Helper.append(log, originalModel.SaleInvoices_Notes, model.SaleInvoices_Notes, UserAccountRolesModel.COL_SaleInvoices_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.SaleInvoices_Add, model.SaleInvoices_Add, UserAccountRolesModel.COL_SaleInvoices_Add.LogDisplay);
                    log = Helper.append(log, originalModel.SaleInvoices_Edit, model.SaleInvoices_Edit, UserAccountRolesModel.COL_SaleInvoices_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.SaleInvoices_View, model.SaleInvoices_View, UserAccountRolesModel.COL_SaleInvoices_View.LogDisplay);
                    log = Helper.append(log, originalModel.SaleInvoices_Approve, model.SaleInvoices_Approve, UserAccountRolesModel.COL_SaleInvoices_Approve.LogDisplay);
                    log = Helper.append(log, originalModel.SaleInvoices_TutorTravelCost_View, model.SaleInvoices_TutorTravelCost_View, UserAccountRolesModel.COL_SaleInvoices_TutorTravelCost_View.LogDisplay);

                    //Payments
                    log = Helper.append(log, originalModel.Payments_Notes, model.Payments_Notes, UserAccountRolesModel.COL_Payments_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Payments_Add, model.Payments_Add, UserAccountRolesModel.COL_Payments_Add.LogDisplay);
                    log = Helper.append(log, originalModel.Payments_Edit, model.Payments_Edit, UserAccountRolesModel.COL_Payments_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.Payments_View, model.Payments_View, UserAccountRolesModel.COL_Payments_View.LogDisplay);
                    log = Helper.append(log, originalModel.Payments_Approve, model.Payments_Approve, UserAccountRolesModel.COL_Payments_Approve.LogDisplay);

                    //PettyCashRecords
                    log = Helper.append(log, originalModel.PettyCashRecords_Notes, model.PettyCashRecords_Notes, UserAccountRolesModel.COL_PettyCashRecords_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.PettyCashRecords_Add, model.PettyCashRecords_Add, UserAccountRolesModel.COL_PettyCashRecords_Add.LogDisplay);
                    log = Helper.append(log, originalModel.PettyCashRecords_Edit, model.PettyCashRecords_Edit, UserAccountRolesModel.COL_PettyCashRecords_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.PettyCashRecords_View, model.PettyCashRecords_View, UserAccountRolesModel.COL_PettyCashRecords_View.LogDisplay);
                    log = Helper.append(log, originalModel.PettyCashRecords_Approve, model.PettyCashRecords_Approve, UserAccountRolesModel.COL_PettyCashRecords_Approve.LogDisplay);

                    //Inventory
                    log = Helper.append(log, originalModel.Inventory_Notes, model.Inventory_Notes, UserAccountRolesModel.COL_Inventory_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Inventory_Add, model.Inventory_Add, UserAccountRolesModel.COL_Inventory_Add.LogDisplay);
                    log = Helper.append(log, originalModel.Inventory_Edit, model.Inventory_Edit, UserAccountRolesModel.COL_Inventory_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.Inventory_View, model.Inventory_View, UserAccountRolesModel.COL_Inventory_View.LogDisplay);

                    //LessonSessions
                    log = Helper.append(log, originalModel.LessonSessions_Notes, model.LessonSessions_Notes, UserAccountRolesModel.COL_LessonSessions_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.LessonSessions_Add, model.LessonSessions_Add, UserAccountRolesModel.COL_LessonSessions_Add.LogDisplay);
                    log = Helper.append(log, originalModel.LessonSessions_Edit, model.LessonSessions_Edit, UserAccountRolesModel.COL_LessonSessions_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.LessonSessions_EditReviewAndInternalNotes, model.LessonSessions_EditReviewAndInternalNotes, UserAccountRolesModel.COL_LessonSessions_EditReviewAndInternalNotes.LogDisplay);
                    log = Helper.append(log, originalModel.LessonSessions_View, model.LessonSessions_View, UserAccountRolesModel.COL_LessonSessions_View.LogDisplay);
                    log = Helper.append(log, originalModel.LessonSessions_InternalNotes_View, model.LessonSessions_InternalNotes_View, UserAccountRolesModel.COL_LessonSessions_InternalNotes_View.LogDisplay);

                    //HourlyRates
                    log = Helper.append(log, originalModel.HourlyRates_Notes, model.HourlyRates_Notes, UserAccountRolesModel.COL_HourlyRates_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.HourlyRates_Add, model.HourlyRates_Add, UserAccountRolesModel.COL_HourlyRates_Add.LogDisplay);
                    log = Helper.append(log, originalModel.HourlyRates_Edit, model.HourlyRates_Edit, UserAccountRolesModel.COL_HourlyRates_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.HourlyRates_View, model.HourlyRates_View, UserAccountRolesModel.COL_HourlyRates_View.LogDisplay);

                    //PayrollPayments
                    log = Helper.append(log, originalModel.PayrollPayments_Notes, model.PayrollPayments_Notes, UserAccountRolesModel.COL_PayrollPayments_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.PayrollPayments_Add, model.PayrollPayments_Add, UserAccountRolesModel.COL_PayrollPayments_Add.LogDisplay);
                    log = Helper.append(log, originalModel.PayrollPayments_Edit, model.PayrollPayments_Edit, UserAccountRolesModel.COL_PayrollPayments_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.PayrollPayments_View, model.PayrollPayments_View, UserAccountRolesModel.COL_PayrollPayments_View.LogDisplay);
                    log = Helper.append(log, originalModel.PayrollPayments_Approve, model.PayrollPayments_Approve, UserAccountRolesModel.COL_PayrollPayments_Approve.LogDisplay);

                    //TutorSchedules
                    log = Helper.append(log, originalModel.TutorSchedules_Notes, model.TutorSchedules_Notes, UserAccountRolesModel.COL_TutorSchedules_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.TutorSchedules_Add, model.TutorSchedules_Add, UserAccountRolesModel.COL_TutorSchedules_Add.LogDisplay);
                    log = Helper.append(log, originalModel.TutorSchedules_Edit, model.TutorSchedules_Edit, UserAccountRolesModel.COL_TutorSchedules_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.TutorSchedules_View, model.TutorSchedules_View, UserAccountRolesModel.COL_TutorSchedules_View.LogDisplay);

                    //StudentSchedules
                    log = Helper.append(log, originalModel.StudentSchedules_Notes, model.StudentSchedules_Notes, UserAccountRolesModel.COL_StudentSchedules_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.StudentSchedules_Add, model.StudentSchedules_Add, UserAccountRolesModel.COL_StudentSchedules_Add.LogDisplay);
                    log = Helper.append(log, originalModel.StudentSchedules_Edit, model.StudentSchedules_Edit, UserAccountRolesModel.COL_StudentSchedules_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.StudentSchedules_View, model.StudentSchedules_View, UserAccountRolesModel.COL_StudentSchedules_View.LogDisplay);

                    //ClubSchedules
                    log = Helper.append(log, originalModel.ClubSchedules_Notes, model.ClubSchedules_Notes, UserAccountRolesModel.COL_ClubSchedules_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.ClubSchedules_Add, model.ClubSchedules_Add, UserAccountRolesModel.COL_ClubSchedules_Add.LogDisplay);
                    log = Helper.append(log, originalModel.ClubSchedules_Edit, model.ClubSchedules_Edit, UserAccountRolesModel.COL_ClubSchedules_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.ClubSchedules_View, model.ClubSchedules_View, UserAccountRolesModel.COL_ClubSchedules_View.LogDisplay);

                    //ClubClasses
                    log = Helper.append(log, originalModel.ClubClasses_Notes, model.ClubClasses_Notes, UserAccountRolesModel.COL_ClubClasses_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.ClubClasses_Add, model.ClubClasses_Add, UserAccountRolesModel.COL_ClubClasses_Add.LogDisplay);
                    log = Helper.append(log, originalModel.ClubClasses_Edit, model.ClubClasses_Edit, UserAccountRolesModel.COL_ClubClasses_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.ClubClasses_View, model.ClubClasses_View, UserAccountRolesModel.COL_ClubClasses_View.LogDisplay);

                    //ClubClassOnlineLinks
                    log = Helper.append(log, originalModel.ClubClassOnlineLinks_Notes, model.ClubClassOnlineLinks_Notes, UserAccountRolesModel.COL_ClubClassOnlineLinks_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.ClubClassOnlineLinks_Add, model.ClubClassOnlineLinks_Add, UserAccountRolesModel.COL_ClubClassOnlineLinks_Add.LogDisplay);
                    log = Helper.append(log, originalModel.ClubClassOnlineLinks_Edit, model.ClubClassOnlineLinks_Edit, UserAccountRolesModel.COL_ClubClassOnlineLinks_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.ClubClassOnlineLinks_View, model.ClubClassOnlineLinks_View, UserAccountRolesModel.COL_ClubClassOnlineLinks_View.LogDisplay);

                    //Files
                    log = Helper.append(log, originalModel.Files_Notes, model.Files_Notes, UserAccountRolesModel.COL_Files_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.Files_Add, model.Files_Add, UserAccountRolesModel.COL_Files_Add.LogDisplay);
                    log = Helper.append(log, originalModel.Files_Edit, model.Files_Edit, UserAccountRolesModel.COL_Files_Edit.LogDisplay);
                    log = Helper.append(log, originalModel.Files_View, model.Files_View, UserAccountRolesModel.COL_Files_View.LogDisplay);
                    log = Helper.append(log, originalModel.Files_EditGlobal, model.Files_EditGlobal, UserAccountRolesModel.COL_Files_EditGlobal.LogDisplay);

                    //Income Statement
                    log = Helper.append(log, originalModel.IncomeStatement_Notes, model.IncomeStatement_Notes, UserAccountRolesModel.COL_IncomeStatement_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.IncomeStatement_View, model.IncomeStatement_View, UserAccountRolesModel.COL_IncomeStatement_View.LogDisplay);
                    log = Helper.append(log, originalModel.IncomeStatement_ViewProfit, model.IncomeStatement_ViewProfit, UserAccountRolesModel.COL_IncomeStatement_ViewProfit.LogDisplay);

                    //LandingPageUpdate
                    log = Helper.append(log, originalModel.LandingPageUpdate_Notes, model.LandingPageUpdate_Notes, UserAccountRolesModel.COL_LandingPageUpdate_Notes.LogDisplay);
                    log = Helper.append(log, originalModel.LandingPageUpdate_Edit, model.LandingPageUpdate_Edit, UserAccountRolesModel.COL_LandingPageUpdate_Edit.LogDisplay);

                    if (!string.IsNullOrEmpty(log))
                    {
                        if (model.Roles_List != null) model.Roles = string.Join(",", Util.removeNullOrEmpty(model.Roles_List).ToArray());
                        db.Entry(model).State = EntityState.Modified;
                        db.SaveChanges();
                        ActivityLogsController.AddEditLog(db, Session, model.Id, log);
                    }

                    UserAccountsController.updateLoginSession(Session);

                    return RedirectToAction(nameof(Index));
                }
            }

            setViewBag(this);
            return View(model);
        }

        /* METHODS ********************************************************************************************************************************************/

        public static void setDropDownListViewBag(Controller controller)
        {
            controller.ViewBag.UserAccountRoles = new SelectList(get(controller.Session), UserAccountRolesModel.COL_Id.Name, UserAccountRolesModel.COL_Name.Name);
        }

        public static void setViewBag(Controller controller)
        {
            //controller.ViewBag.UserAccountRolesModels = get();
            UserAccountRolesController.setDropDownListViewBag(controller);
        }

        public static UserAccountRolesModel getAccesses(UserAccountsModel UserAccount) 
        {
            UserAccountRolesModel model = new UserAccountRolesModel();
            foreach(UserAccountRolesModel item in get(UserAccount.Roles))
            {
                if (model.Roles_List == null && item.Roles_List != null && item.Roles_List.Count > 0) model.Roles_List = new List<string>();
                if(item.Roles_List != null && item.Roles_List.Count > 0) model.Roles_List.AddRange(item.Roles_List);

                //ActivityLogs
                if (item.ActivityLogs_View) model.ActivityLogs_View = true;

                //Reminders
                if (item.Reminders_Add) model.Reminders_Add = true;
                if (item.Reminders_Edit) model.Reminders_Edit = true;
                if (item.Reminders_View) model.Reminders_View = true;

                //Birthdays
                if (item.Birthdays_View) model.Birthdays_View = true;

                //UserAccounts
                if (item.UserAccounts_Add) model.UserAccounts_Add = true;
                if (item.UserAccounts_Edit) model.UserAccounts_Edit = true;
                if (item.UserAccounts_View) model.UserAccounts_View = true;
                if (item.UserAccounts_ResetPassword) model.UserAccounts_ResetPassword = true;
                if (item.UserAccounts_ViewAllRoles) model.UserAccounts_ViewAllRoles = true;

                //UserAccountRoles
                if (item.UserAccountRoles_Add) model.UserAccountRoles_Add = true;
                if (item.UserAccountRoles_Edit) model.UserAccountRoles_Edit = true;
                if (item.UserAccountRoles_View) model.UserAccountRoles_View = true;

                //Settings
                if (item.Settings_Edit) model.Settings_Edit = true;
                if (item.Settings_View) model.Settings_View = true;

                //Franchises
                if (item.Franchises_Add) model.Franchises_Add = true;
                if (item.Franchises_Edit) model.Franchises_Edit = true;
                if (item.Franchises_View) model.Franchises_View = true;

                //Branches
                if (item.Branches_Add) model.Branches_Add = true;
                if (item.Branches_Edit) model.Branches_Edit = true;
                if (item.Branches_View) model.Branches_View = true;

                //PromotionEvents
                if (item.PromotionEvents_Add) model.PromotionEvents_Add = true;
                if (item.PromotionEvents_Edit) model.PromotionEvents_Edit = true;
                if (item.PromotionEvents_View) model.PromotionEvents_View = true;

                //PettyCashRecordsCategories
                if (item.PettyCashRecordsCategories_Add) model.PettyCashRecordsCategories_Add = true;
                if (item.PettyCashRecordsCategories_Edit) model.PettyCashRecordsCategories_Edit = true;
                if (item.PettyCashRecordsCategories_View) model.PettyCashRecordsCategories_View = true;

                //Languages
                if (item.Languages_Add) model.Languages_Add = true;
                if (item.Languages_Edit) model.Languages_Edit = true;
                if (item.Languages_View) model.Languages_View = true;

                //LessonTypes
                if (item.LessonTypes_Add) model.LessonTypes_Add = true;
                if (item.LessonTypes_Edit) model.LessonTypes_Edit = true;
                if (item.LessonTypes_View) model.LessonTypes_View = true;

                //LessonPackages
                if (item.LessonPackages_Add) model.LessonPackages_Add = true;
                if (item.LessonPackages_Edit) model.LessonPackages_Edit = true;
                if (item.LessonPackages_View) model.LessonPackages_View = true;

                //Consignments
                if (item.Consignments_Add) model.Consignments_Add = true;
                if (item.Consignments_Edit) model.Consignments_Edit = true;
                if (item.Consignments_View) model.Consignments_View = true;

                //Vouchers
                if (item.Vouchers_Add) model.Vouchers_Add = true;
                if (item.Vouchers_Edit) model.Vouchers_Edit = true;
                if (item.Vouchers_View) model.Vouchers_View = true;

                //Suppliers
                if (item.Suppliers_Add) model.Suppliers_Add = true;
                if (item.Suppliers_Edit) model.Suppliers_Edit = true;
                if (item.Suppliers_View) model.Suppliers_View = true;

                //Units
                if (item.Units_Add) model.Units_Add = true;
                if (item.Units_Edit) model.Units_Edit = true;
                if (item.Units_View) model.Units_View = true;

                //ExpenseCategories
                if (item.ExpenseCategories_Add) model.ExpenseCategories_Add = true;
                if (item.ExpenseCategories_Edit) model.ExpenseCategories_Edit = true;
                if (item.ExpenseCategories_View) model.ExpenseCategories_View = true;

                //Services
                if (item.Services_Add) model.Services_Add = true;
                if (item.Services_Edit) model.Services_Edit = true;
                if (item.Services_View) model.Services_View = true;

                //Products
                if (item.Products_Add) model.Products_Add = true;
                if (item.Products_Edit) model.Products_Edit = true;
                if (item.Products_View) model.Products_View = true;

                //SaleInvoices
                if (item.SaleInvoices_Add) model.SaleInvoices_Add = true;
                if (item.SaleInvoices_Edit) model.SaleInvoices_Edit = true;
                if (item.SaleInvoices_View) model.SaleInvoices_View = true;
                if (item.SaleInvoices_Approve) model.SaleInvoices_Approve = true;
                if (item.SaleInvoices_TutorTravelCost_View) model.SaleInvoices_TutorTravelCost_View = true;

                //Payments
                if (item.Payments_Add) model.Payments_Add = true;
                if (item.Payments_Edit) model.Payments_Edit = true;
                if (item.Payments_View) model.Payments_View = true;
                if (item.Payments_Approve) model.Payments_Approve = true;

                //PettyCashRecords
                if (item.PettyCashRecords_Add) model.PettyCashRecords_Add = true;
                if (item.PettyCashRecords_Edit) model.PettyCashRecords_Edit = true;
                if (item.PettyCashRecords_View) model.PettyCashRecords_View = true;
                if (item.PettyCashRecords_Approve) model.PettyCashRecords_Approve = true;

                //Inventory
                if (item.Inventory_Add) model.Inventory_Add = true;
                if (item.Inventory_Edit) model.Inventory_Edit = true;
                if (item.Inventory_View) model.Inventory_View = true;

                //LessonSessions
                if (item.LessonSessions_Add) model.LessonSessions_Add = true;
                if (item.LessonSessions_Edit) model.LessonSessions_Edit = true;
                if (item.LessonSessions_EditReviewAndInternalNotes) model.LessonSessions_EditReviewAndInternalNotes = true;
                if (item.LessonSessions_View) model.LessonSessions_View = true;
                if (item.LessonSessions_InternalNotes_View) model.LessonSessions_InternalNotes_View = true;

                //HourlyRates
                if (item.HourlyRates_Add) model.HourlyRates_Add = true;
                if (item.HourlyRates_Edit) model.HourlyRates_Edit = true;
                if (item.HourlyRates_View) model.HourlyRates_View = true;

                //PayrollPayments
                if (item.PayrollPayments_Add) model.PayrollPayments_Add = true;
                if (item.PayrollPayments_Edit) model.PayrollPayments_Edit = true;
                if (item.PayrollPayments_View) model.PayrollPayments_View = true;
                if (item.PayrollPayments_Approve) model.PayrollPayments_Approve = true;

                //TutorSchedules
                if (item.TutorSchedules_Add) model.TutorSchedules_Add = true;
                if (item.TutorSchedules_Edit) model.TutorSchedules_Edit = true;
                if (item.TutorSchedules_View) model.TutorSchedules_View = true;

                //StudentSchedules
                if (item.StudentSchedules_Add) model.StudentSchedules_Add = true;
                if (item.StudentSchedules_Edit) model.StudentSchedules_Edit = true;
                if (item.StudentSchedules_View) model.StudentSchedules_View = true;

                //ClubSchedules
                if (item.ClubSchedules_Add) model.ClubSchedules_Add = true;
                if (item.ClubSchedules_Edit) model.ClubSchedules_Edit = true;
                if (item.ClubSchedules_View) model.ClubSchedules_View = true;

                //ClubClasses
                if (item.ClubClasses_Add) model.ClubClasses_Add = true;
                if (item.ClubClasses_Edit) model.ClubClasses_Edit = true;
                if (item.ClubClasses_View) model.ClubClasses_View = true;

                //ClubClassOnlineLinks
                if (item.ClubClassOnlineLinks_Add) model.ClubClassOnlineLinks_Add = true;
                if (item.ClubClassOnlineLinks_Edit) model.ClubClassOnlineLinks_Edit = true;
                if (item.ClubClassOnlineLinks_View) model.ClubClassOnlineLinks_View = true;

                //Files
                if (item.Files_Add) model.Files_Add = true;
                if (item.Files_Edit) model.Files_Edit = true;
                if (item.Files_View) model.Files_View = true;
                if (item.Files_EditGlobal) model.Files_EditGlobal = true;

                //Income Statement
                if (item.IncomeStatement_View) model.IncomeStatement_View = true;
                if (item.IncomeStatement_ViewProfit) model.IncomeStatement_ViewProfit = true;

                //LandingPageUpdate
                if (item.LandingPageUpdate_Edit) model.LandingPageUpdate_Edit = true;

            }

            if (model.Roles_List != null) model.Roles = string.Join(",", model.Roles_List.ToArray());

            return model;
        }

        /* DATABASE METHODS ***********************************************************************************************************************************/

        public bool isExists(EnumActions action, Guid? id, object value)
        {
            var result = action == EnumActions.Create
                ? db.UserAccountRoles.AsNoTracking().Where(x => x.Name.ToLower() == value.ToString().ToLower()).FirstOrDefault()
                : db.UserAccountRoles.AsNoTracking().Where(x => x.Name.ToLower() == value.ToString().ToLower() && x.Id != id).FirstOrDefault();
            return result != null;
        }

        public static List<UserAccountRolesModel> get(HttpSessionStateBase Session) { return get(null, UserAccountsController.getUserAccess(Session).Roles, UserAccountsController.getUserAccess(Session).UserAccounts_ViewAllRoles); }
        public static List<UserAccountRolesModel> get(string UserAccountRoles) { return get(null, UserAccountRoles, false); }
        public UserAccountRolesModel get(Guid? Id) { return get(Id, null, true).FirstOrDefault(); }
        public static List<UserAccountRolesModel> get(Guid? Id, string UserAccountRoles, bool UserAccounts_ViewAllRoles)
        {
            string UserAccountRolesClause = "";
            if (!string.IsNullOrEmpty(UserAccountRoles) && !UserAccounts_ViewAllRoles)
                UserAccountRolesClause = string.Format(" AND UserAccountRoles.Id IN ({0}) ", LIBWebMVC.UtilWebMVC.convertToSqlIdList(UserAccountRoles));

            string sql = string.Format(@"
                        SELECT UserAccountRoles.*
                        FROM UserAccountRoles
                        WHERE 1=1
							AND (@Id IS NULL OR UserAccountRoles.Id = @Id)
                            AND (@Id IS NOT NULL OR (
                                1=1
                                {0}
                            ))
						ORDER BY UserAccountRoles.Name ASC
                    ", UserAccountRolesClause);

            List<UserAccountRolesModel> models = new DBContext().Database.SqlQuery<UserAccountRolesModel>(sql,
                    DBConnection.getSqlParameter(UserAccountRolesModel.COL_Id.Name, Id)
                ).ToList();

            foreach (UserAccountRolesModel model in models)
            {
                if (!string.IsNullOrEmpty(model.Roles)) model.Roles_List = model.Roles.Split(',').ToList();
            }
            return models;
        }

        /******************************************************************************************************************************************************/
    }
}