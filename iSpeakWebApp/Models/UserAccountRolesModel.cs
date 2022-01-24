using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    [Table("UserAccountRoles")]
    public class UserAccountRolesModel
    {
        [Key]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };

        public string Name { get; set; }
        public static ModelMember COL_Name = new ModelMember { Name = "Name", Display = "Name", LogDisplay = ActivityLogsController.editStringFormat("Name") };

        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Notes") };

        /* REMINDERS ******************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string Reminders_Notes { get; set; }
        public static ModelMember COL_Reminders_Notes = new ModelMember { Name = "Reminders_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Reminders Notes") };

        [Display(Name = "Add")]
        public bool Reminders_Add { get; set; }
        public static ModelMember COL_Reminders_Add = new ModelMember { Name = "Reminders_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("Reminders Add") };

        [Display(Name = "View")]
        public bool Reminders_View { get; set; }
        public static ModelMember COL_Reminders_View = new ModelMember { Name = "Reminders_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("Reminders View") };

        [Display(Name = "Edit")]
        public bool Reminders_Edit { get; set; }
        public static ModelMember COL_Reminders_Edit = new ModelMember { Name = "Reminders_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("Reminders Edit") };


        /* BIRTHDAYS ******************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string Birthdays_Notes { get; set; }
        public static ModelMember COL_Birthdays_Notes = new ModelMember { Name = "Birthdays_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Birthdays Notes") };

        [Display(Name = "View")]
        public bool Birthdays_View { get; set; }
        public static ModelMember COL_Birthdays_View = new ModelMember { Name = "Birthdays_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("Birthdays View") };


        /* USER ACCOUNTS **************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string UserAccounts_Notes { get; set; }
        public static ModelMember COL_UserAccounts_Notes = new ModelMember { Name = "UserAccounts_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("UserAccounts Notes") };

        [Display(Name = "Add")]
        public bool UserAccounts_Add { get; set; }
        public static ModelMember COL_UserAccounts_Add = new ModelMember { Name = "UserAccounts_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("UserAccounts Add") };

        [Display(Name = "View")]
        public bool UserAccounts_View { get; set; }
        public static ModelMember COL_UserAccounts_View = new ModelMember { Name = "UserAccounts_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("UserAccounts View") };

        [Display(Name = "Edit")]
        public bool UserAccounts_Edit { get; set; }
        public static ModelMember COL_UserAccounts_Edit = new ModelMember { Name = "UserAccounts_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("UserAccounts Edit") };

        [Display(Name = "Reset Password")]
        public bool UserAccounts_ResetPassword { get; set; }
        public static ModelMember COL_UserAccounts_ResetPassword = new ModelMember { Name = "UserAccounts_ResetPassword", Display = "Reset Password", LogDisplay = ActivityLogsController.editBooleanFormat("UserAccounts Reset Password") };

        [Display(Name = "Edit Roles")]
        public bool UserAccounts_EditRoles { get; set; }
        public static ModelMember COL_UserAccounts_EditRoles = new ModelMember { Name = "UserAccounts_EditRoles", Display = "Edit Roles", LogDisplay = ActivityLogsController.editBooleanFormat("UserAccounts Edit Roles") };


        /* USER ACCOUNTS ROLES ********************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string UserAccountRoles_Notes { get; set; }
        public static ModelMember COL_UserAccountRoles_Notes = new ModelMember { Name = "UserAccountRoles_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Branches Notes") };

        [Display(Name = "Add")]
        public bool UserAccountRoles_Add { get; set; }
        public static ModelMember COL_UserAccountRoles_Add = new ModelMember { Name = "UserAccountRoles_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("Branches Add") };

        [Display(Name = "View")]
        public bool UserAccountRoles_View { get; set; }
        public static ModelMember COL_UserAccountRoles_View = new ModelMember { Name = "UserAccountRoles_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("Branches View") };

        [Display(Name = "Edit")]
        public bool UserAccountRoles_Edit { get; set; }
        public static ModelMember COL_UserAccountRoles_Edit = new ModelMember { Name = "UserAccountRoles_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("Branches Edit") };


        /* APPLICATION SETTINGS *******************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string Settings_Notes { get; set; }
        public static ModelMember COL_Settings_Notes = new ModelMember { Name = "Settings_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Settings Roles Notes") };

        [Display(Name = "View")]
        public bool Settings_View { get; set; }
        public static ModelMember COL_Settings_View = new ModelMember { Name = "Settings_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("Settings Roles View") };

        [Display(Name = "Edit")]
        public bool Settings_Edit { get; set; }
        public static ModelMember COL_Settings_Edit = new ModelMember { Name = "Settings_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("Settings Roles Edit") };

        /* BRANCHES *******************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string Branches_Notes { get; set; }
        public static ModelMember COL_Branches_Notes = new ModelMember { Name = "Branches_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Branches Notes") };

        [Display(Name = "Add")]
        public bool Branches_Add { get; set; }
        public static ModelMember COL_Branches_Add = new ModelMember { Name = "Branches_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("Branches Add") };

        [Display(Name = "View")]
        public bool Branches_View { get; set; }
        public static ModelMember COL_Branches_View = new ModelMember { Name = "Branches_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("Branches View") };

        [Display(Name = "Edit")]
        public bool Branches_Edit { get; set; }
        public static ModelMember COL_Branches_Edit = new ModelMember { Name = "Branches_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("Branches Edit") };


        /* PROMOTION EVENTS ***********************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string PromotionEvents_Notes { get; set; }
        public static ModelMember COL_PromotionEvents_Notes = new ModelMember { Name = "PromotionEvents_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("PromotionEvents Notes") };

        [Display(Name = "Add")]
        public bool PromotionEvents_Add { get; set; }
        public static ModelMember COL_PromotionEvents_Add = new ModelMember { Name = "PromotionEvents_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("PromotionEvents Add") };

        [Display(Name = "View")]
        public bool PromotionEvents_View { get; set; }
        public static ModelMember COL_PromotionEvents_View = new ModelMember { Name = "PromotionEvents_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("PromotionEvents View") };

        [Display(Name = "Edit")]
        public bool PromotionEvents_Edit { get; set; }
        public static ModelMember COL_PromotionEvents_Edit = new ModelMember { Name = "PromotionEvents_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("PromotionEvents Edit") };


        /* PETTY CASH RECORDS CATEGORIES **********************************************************************************************************************/

        [Display(Name = "Notes")]
        public string PettyCashRecordsCategories_Notes { get; set; }
        public static ModelMember COL_PettyCashRecordsCategories_Notes = new ModelMember { Name = "PettyCashRecordsCategories_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("PettyCashRecordsCategories Notes") };

        [Display(Name = "Add")]
        public bool PettyCashRecordsCategories_Add { get; set; }
        public static ModelMember COL_PettyCashRecordsCategories_Add = new ModelMember { Name = "PettyCashRecordsCategories_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("PettyCashRecordsCategories Add") };

        [Display(Name = "View")]
        public bool PettyCashRecordsCategories_View { get; set; }
        public static ModelMember COL_PettyCashRecordsCategories_View = new ModelMember { Name = "PettyCashRecordsCategories_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("PettyCashRecordsCategories View") };

        [Display(Name = "Edit")]
        public bool PettyCashRecordsCategories_Edit { get; set; }
        public static ModelMember COL_PettyCashRecordsCategories_Edit = new ModelMember { Name = "PettyCashRecordsCategories_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("PettyCashRecordsCategories Edit") };

        /* LANGUAGES ******************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string Languages_Notes { get; set; }
        public static ModelMember COL_Languages_Notes = new ModelMember { Name = "Languages_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Languages Notes") };

        [Display(Name = "Add")]
        public bool Languages_Add { get; set; }
        public static ModelMember COL_Languages_Add = new ModelMember { Name = "Languages_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("Languages Add") };

        [Display(Name = "View")]
        public bool Languages_View { get; set; }
        public static ModelMember COL_Languages_View = new ModelMember { Name = "Languages_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("Languages View") };

        [Display(Name = "Edit")]
        public bool Languages_Edit { get; set; }
        public static ModelMember COL_Languages_Edit = new ModelMember { Name = "Languages_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("Languages Edit") };

        /* LESSON TYPES ***************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string LessonTypes_Notes { get; set; }
        public static ModelMember COL_LessonTypes_Notes = new ModelMember { Name = "LessonTypes_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("LessonTypes Notes") };

        [Display(Name = "Add")]
        public bool LessonTypes_Add { get; set; }
        public static ModelMember COL_LessonTypes_Add = new ModelMember { Name = "LessonTypes_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("LessonTypes Add") };

        [Display(Name = "View")]
        public bool LessonTypes_View { get; set; }
        public static ModelMember COL_LessonTypes_View = new ModelMember { Name = "LessonTypes_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("LessonTypes View") };

        [Display(Name = "Edit")]
        public bool LessonTypes_Edit { get; set; }
        public static ModelMember COL_LessonTypes_Edit = new ModelMember { Name = "LessonTypes_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("LessonTypes Edit") };

        /* LESSON PACKAGES ************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string LessonPackages_Notes { get; set; }
        public static ModelMember COL_LessonPackages_Notes = new ModelMember { Name = "LessonPackages_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("LessonPackages Notes") };

        [Display(Name = "Add")]
        public bool LessonPackages_Add { get; set; }
        public static ModelMember COL_LessonPackages_Add = new ModelMember { Name = "LessonPackages_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("LessonPackages Add") };

        [Display(Name = "View")]
        public bool LessonPackages_View { get; set; }
        public static ModelMember COL_LessonPackages_View = new ModelMember { Name = "LessonPackages_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("LessonPackages View") };

        [Display(Name = "Edit")]
        public bool LessonPackages_Edit { get; set; }
        public static ModelMember COL_LessonPackages_Edit = new ModelMember { Name = "LessonPackages_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("LessonPackages Edit") };

        /* CONSIGNMENTS ***************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string Consignments_Notes { get; set; }
        public static ModelMember COL_Consignments_Notes = new ModelMember { Name = "Consignments_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Consignments Notes") };

        [Display(Name = "Add")]
        public bool Consignments_Add { get; set; }
        public static ModelMember COL_Consignments_Add = new ModelMember { Name = "Consignments_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("Consignments Add") };

        [Display(Name = "View")]
        public bool Consignments_View { get; set; }
        public static ModelMember COL_Consignments_View = new ModelMember { Name = "Consignments_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("Consignments View") };

        [Display(Name = "Edit")]
        public bool Consignments_Edit { get; set; }
        public static ModelMember COL_Consignments_Edit = new ModelMember { Name = "Consignments_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("Consignments Edit") };

        /* VOUCHERS *******************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string Vouchers_Notes { get; set; }
        public static ModelMember COL_Vouchers_Notes = new ModelMember { Name = "Vouchers_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Vouchers Notes") };

        [Display(Name = "Add")]
        public bool Vouchers_Add { get; set; }
        public static ModelMember COL_Vouchers_Add = new ModelMember { Name = "Vouchers_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("Vouchers Add") };

        [Display(Name = "View")]
        public bool Vouchers_View { get; set; }
        public static ModelMember COL_Vouchers_View = new ModelMember { Name = "Vouchers_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("Vouchers View") };

        [Display(Name = "Edit")]
        public bool Vouchers_Edit { get; set; }
        public static ModelMember COL_Vouchers_Edit = new ModelMember { Name = "Vouchers_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("Vouchers Edit") };

        /* SUPPLIERS ******************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string Suppliers_Notes { get; set; }
        public static ModelMember COL_Suppliers_Notes = new ModelMember { Name = "Suppliers_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Suppliers Notes") };

        [Display(Name = "Add")]
        public bool Suppliers_Add { get; set; }
        public static ModelMember COL_Suppliers_Add = new ModelMember { Name = "Suppliers_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("Suppliers Add") };

        [Display(Name = "View")]
        public bool Suppliers_View { get; set; }
        public static ModelMember COL_Suppliers_View = new ModelMember { Name = "Suppliers_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("Suppliers View") };

        [Display(Name = "Edit")]
        public bool Suppliers_Edit { get; set; }
        public static ModelMember COL_Suppliers_Edit = new ModelMember { Name = "Suppliers_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("Suppliers Edit") };

        /* UNITS **********************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string Units_Notes { get; set; }
        public static ModelMember COL_Units_Notes = new ModelMember { Name = "Units_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Units Notes") };

        [Display(Name = "Add")]
        public bool Units_Add { get; set; }
        public static ModelMember COL_Units_Add = new ModelMember { Name = "Units_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("Units Add") };

        [Display(Name = "View")]
        public bool Units_View { get; set; }
        public static ModelMember COL_Units_View = new ModelMember { Name = "Units_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("Units View") };

        [Display(Name = "Edit")]
        public bool Units_Edit { get; set; }
        public static ModelMember COL_Units_Edit = new ModelMember { Name = "Units_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("Units Edit") };

        /* EXPENSE CATEGORIES *********************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string ExpenseCategories_Notes { get; set; }
        public static ModelMember COL_ExpenseCategories_Notes = new ModelMember { Name = "ExpenseCategories_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("ExpenseCategories Notes") };

        [Display(Name = "Add")]
        public bool ExpenseCategories_Add { get; set; }
        public static ModelMember COL_ExpenseCategories_Add = new ModelMember { Name = "ExpenseCategories_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("ExpenseCategories Add") };

        [Display(Name = "View")]
        public bool ExpenseCategories_View { get; set; }
        public static ModelMember COL_ExpenseCategories_View = new ModelMember { Name = "ExpenseCategories_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("ExpenseCategories View") };

        [Display(Name = "Edit")]
        public bool ExpenseCategories_Edit { get; set; }
        public static ModelMember COL_ExpenseCategories_Edit = new ModelMember { Name = "ExpenseCategories_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("ExpenseCategories Edit") };

        /* SERVICES *******************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string Services_Notes { get; set; }
        public static ModelMember COL_Services_Notes = new ModelMember { Name = "Services_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Services Notes") };

        [Display(Name = "Add")]
        public bool Services_Add { get; set; }
        public static ModelMember COL_Services_Add = new ModelMember { Name = "Services_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("Services Add") };

        [Display(Name = "View")]
        public bool Services_View { get; set; }
        public static ModelMember COL_Services_View = new ModelMember { Name = "Services_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("Services View") };

        [Display(Name = "Edit")]
        public bool Services_Edit { get; set; }
        public static ModelMember COL_Services_Edit = new ModelMember { Name = "Services_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("Services Edit") };

        /* PRODUCTS *******************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string Products_Notes { get; set; }
        public static ModelMember COL_Products_Notes = new ModelMember { Name = "Products_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Products Notes") };

        [Display(Name = "Add")]
        public bool Products_Add { get; set; }
        public static ModelMember COL_Products_Add = new ModelMember { Name = "Products_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("Products Add") };

        [Display(Name = "View")]
        public bool Products_View { get; set; }
        public static ModelMember COL_Products_View = new ModelMember { Name = "Products_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("Products View") };

        [Display(Name = "Edit")]
        public bool Products_Edit { get; set; }
        public static ModelMember COL_Products_Edit = new ModelMember { Name = "Products_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("Products Edit") };

        /* SALE INVOICES **************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string SaleInvoices_Notes { get; set; }
        public static ModelMember COL_SaleInvoices_Notes = new ModelMember { Name = "SaleInvoices_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("SaleInvoices Notes") };

        [Display(Name = "Add")]
        public bool SaleInvoices_Add { get; set; }
        public static ModelMember COL_SaleInvoices_Add = new ModelMember { Name = "SaleInvoices_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("SaleInvoices Add") };

        [Display(Name = "View")]
        public bool SaleInvoices_View { get; set; }
        public static ModelMember COL_SaleInvoices_View = new ModelMember { Name = "SaleInvoices_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("SaleInvoices View") };

        [Display(Name = "Edit")]
        public bool SaleInvoices_Edit { get; set; }
        public static ModelMember COL_SaleInvoices_Edit = new ModelMember { Name = "SaleInvoices_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("SaleInvoices Edit") };

        [Display(Name = "Approve")]
        public bool SaleInvoices_Approve { get; set; }
        public static ModelMember COL_SaleInvoices_Approve = new ModelMember { Name = "SaleInvoices_Approve", Display = "Approve", LogDisplay = ActivityLogsController.editBooleanFormat("SaleInvoices Approve") };

        [Display(Name = "View Tutor Travel Cost")]
        public bool SaleInvoices_TutorTravelCost_View { get; set; }
        public static ModelMember COL_SaleInvoices_TutorTravelCost_View = new ModelMember { Name = "SaleInvoices_TutorTravelCost_View", Display = "View Tutor Travel Cost", LogDisplay = ActivityLogsController.editBooleanFormat("SaleInvoices Tutor Travel Cost View") };

        /* SALE PAYMENTS **************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string Payments_Notes { get; set; }
        public static ModelMember COL_Payments_Notes = new ModelMember { Name = "Payments_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Payments Notes") };

        [Display(Name = "Add")]
        public bool Payments_Add { get; set; }
        public static ModelMember COL_Payments_Add = new ModelMember { Name = "Payments_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("Payments Add") };

        [Display(Name = "View")]
        public bool Payments_View { get; set; }
        public static ModelMember COL_Payments_View = new ModelMember { Name = "Payments_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("Payments View") };

        [Display(Name = "Edit")]
        public bool Payments_Edit { get; set; }
        public static ModelMember COL_Payments_Edit = new ModelMember { Name = "Payments_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("Payments Edit") };

        [Display(Name = "Approve")]
        public bool Payments_Approve { get; set; }
        public static ModelMember COL_Payments_Approve = new ModelMember { Name = "Payments_Approve", Display = "Approve", LogDisplay = ActivityLogsController.editBooleanFormat("Payments Approve") };

        /* PETTY CASH *****************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string PettyCashRecords_Notes { get; set; }
        public static ModelMember COL_PettyCashRecords_Notes = new ModelMember { Name = "PettyCashRecords_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("PettyCashRecords Notes") };

        [Display(Name = "Add")]
        public bool PettyCashRecords_Add { get; set; }
        public static ModelMember COL_PettyCashRecords_Add = new ModelMember { Name = "PettyCashRecords_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("PettyCashRecords Add") };

        [Display(Name = "View")]
        public bool PettyCashRecords_View { get; set; }
        public static ModelMember COL_PettyCashRecords_View = new ModelMember { Name = "PettyCashRecords_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("PettyCashRecords View") };

        [Display(Name = "Edit")]
        public bool PettyCashRecords_Edit { get; set; }
        public static ModelMember COL_PettyCashRecords_Edit = new ModelMember { Name = "PettyCashRecords_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("PettyCashRecords Edit") };

        [Display(Name = "Approve")]
        public bool PettyCashRecords_Approve { get; set; }
        public static ModelMember COL_PettyCashRecords_Approve = new ModelMember { Name = "PettyCashRecords_Approve", Display = "Approve", LogDisplay = ActivityLogsController.editBooleanFormat("PettyCashRecords Approve") };

        /* INVENTORY ******************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string Inventory_Notes { get; set; }
        public static ModelMember COL_Inventory_Notes = new ModelMember { Name = "Inventory_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Inventory Notes") };

        [Display(Name = "Add")]
        public bool Inventory_Add { get; set; }
        public static ModelMember COL_Inventory_Add = new ModelMember { Name = "Inventory_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("Inventory Add") };

        [Display(Name = "View")]
        public bool Inventory_View { get; set; }
        public static ModelMember COL_Inventory_View = new ModelMember { Name = "Inventory_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("Inventory View") };

        [Display(Name = "Edit")]
        public bool Inventory_Edit { get; set; }
        public static ModelMember COL_Inventory_Edit = new ModelMember { Name = "Inventory_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("Inventory Edit") };

        /* LESSON SESSIONS ************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string LessonSessions_Notes { get; set; }
        public static ModelMember COL_LessonSessions_Notes = new ModelMember { Name = "LessonSessions_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Lesson Sessions Notes") };

        [Display(Name = "Add")]
        public bool LessonSessions_Add { get; set; }
        public static ModelMember COL_LessonSessions_Add = new ModelMember { Name = "LessonSessions_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("Lesson Sessions Add") };

        [Display(Name = "View")]
        public bool LessonSessions_View { get; set; }
        public static ModelMember COL_LessonSessions_View = new ModelMember { Name = "LessonSessions_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("Lesson Sessions View") };

        [Display(Name = "Edit All")]
        public bool LessonSessions_Edit { get; set; }
        public static ModelMember COL_LessonSessions_Edit = new ModelMember { Name = "LessonSessions_Edit", Display = "Edit All", LogDisplay = ActivityLogsController.editBooleanFormat("Lesson Sessions Edit All") };

        [Display(Name = "Edit Review & Internal Notes")]
        public bool LessonSessions_EditReviewAndInternalNotes { get; set; }
        public static ModelMember COL_LessonSessions_EditReviewAndInternalNotes = new ModelMember { Name = "LessonSessions_EditReviewAndInternalNotes", Display = "Edit Review & Internal Notes", LogDisplay = ActivityLogsController.editBooleanFormat("Lesson Sessions Edit Review & Internal Notes") };

        [Display(Name = "View Internal Notes")]
        public bool LessonSessions_InternalNotes_View { get; set; }
        public static ModelMember COL_LessonSessions_InternalNotes_View = new ModelMember { Name = "LessonSessions_InternalNotes_View", Display = "View Internal Notes", LogDisplay = ActivityLogsController.editBooleanFormat("Lesson Sessions Internal Notes View") };

        /* HOURLY RATES ***************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string HourlyRates_Notes { get; set; }
        public static ModelMember COL_HourlyRates_Notes = new ModelMember { Name = "HourlyRates_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("HourlyRates Notes") };

        [Display(Name = "Add")]
        public bool HourlyRates_Add { get; set; }
        public static ModelMember COL_HourlyRates_Add = new ModelMember { Name = "HourlyRates_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("HourlyRates Add") };

        [Display(Name = "View")]
        public bool HourlyRates_View { get; set; }
        public static ModelMember COL_HourlyRates_View = new ModelMember { Name = "HourlyRates_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("HourlyRates View") };

        [Display(Name = "Edit")]
        public bool HourlyRates_Edit { get; set; }
        public static ModelMember COL_HourlyRates_Edit = new ModelMember { Name = "HourlyRates_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("HourlyRates Edit") };

        /* PAYROLL PAYMENTS ***********************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string PayrollPayments_Notes { get; set; }
        public static ModelMember COL_PayrollPayments_Notes = new ModelMember { Name = "PayrollPayments_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("PayrollPayments Notes") };

        [Display(Name = "Add")]
        public bool PayrollPayments_Add { get; set; }
        public static ModelMember COL_PayrollPayments_Add = new ModelMember { Name = "PayrollPayments_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("PayrollPayments Add") };

        [Display(Name = "View")]
        public bool PayrollPayments_View { get; set; }
        public static ModelMember COL_PayrollPayments_View = new ModelMember { Name = "PayrollPayments_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("PayrollPayments View") };

        [Display(Name = "Edit")]
        public bool PayrollPayments_Edit { get; set; }
        public static ModelMember COL_PayrollPayments_Edit = new ModelMember { Name = "PayrollPayments_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("PayrollPayments Edit") };

        [Display(Name = "Approve")]
        public bool PayrollPayments_Approve { get; set; }
        public static ModelMember COL_PayrollPayments_Approve = new ModelMember { Name = "PayrollPayments_Approve", Display = "Approve", LogDisplay = ActivityLogsController.editBooleanFormat("PayrollPayments Approve") };

        /* TUTOR SCHEDULES ************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string TutorSchedules_Notes { get; set; }
        public static ModelMember COL_TutorSchedules_Notes = new ModelMember { Name = "TutorSchedules_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("TutorSchedules Notes") };

        [Display(Name = "Add")]
        public bool TutorSchedules_Add { get; set; }
        public static ModelMember COL_TutorSchedules_Add = new ModelMember { Name = "TutorSchedules_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("TutorSchedules Add") };

        [Display(Name = "View")]
        public bool TutorSchedules_View { get; set; }
        public static ModelMember COL_TutorSchedules_View = new ModelMember { Name = "TutorSchedules_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("TutorSchedules View") };

        [Display(Name = "Edit")]
        public bool TutorSchedules_Edit { get; set; }
        public static ModelMember COL_TutorSchedules_Edit = new ModelMember { Name = "TutorSchedules_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("TutorSchedules Edit") };

        /* STUDENT SCHEDULES **********************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string StudentSchedules_Notes { get; set; }
        public static ModelMember COL_StudentSchedules_Notes = new ModelMember { Name = "StudentSchedules_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("StudentSchedules Notes") };

        [Display(Name = "Add")]
        public bool StudentSchedules_Add { get; set; }
        public static ModelMember COL_StudentSchedules_Add = new ModelMember { Name = "StudentSchedules_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("StudentSchedules Add") };

        [Display(Name = "View")]
        public bool StudentSchedules_View { get; set; }
        public static ModelMember COL_StudentSchedules_View = new ModelMember { Name = "StudentSchedules_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("StudentSchedules View") };

        [Display(Name = "Edit")]
        public bool StudentSchedules_Edit { get; set; }
        public static ModelMember COL_StudentSchedules_Edit = new ModelMember { Name = "StudentSchedules_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("StudentSchedules Edit") };

        /******************************************************************************************************************************************************/

    }
}