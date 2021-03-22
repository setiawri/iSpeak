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
        public static ModelMember COL_Reminders_Notes = new ModelMember { Name = "Reminders_Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Reminder Notes") };

        [Display(Name = "Add")]
        public bool Reminders_Add { get; set; }
        public static ModelMember COL_Reminders_Add = new ModelMember { Name = "Reminders_Add", Display = "Add", LogDisplay = ActivityLogsController.editBooleanFormat("Reminder Add") };

        [Display(Name = "View")]
        public bool Reminders_View { get; set; }
        public static ModelMember COL_Reminders_View = new ModelMember { Name = "Reminders_View", Display = "View", LogDisplay = ActivityLogsController.editBooleanFormat("Reminder View") };

        [Display(Name = "Edit")]
        public bool Reminders_Edit { get; set; }
        public static ModelMember COL_Reminders_Edit = new ModelMember { Name = "Reminders_Edit", Display = "Edit", LogDisplay = ActivityLogsController.editBooleanFormat("Reminder Edit") };


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

        [Display(Name = "View Tutor Travel Cost")]
        public bool SaleInvoices_TutorTravelCost_View { get; set; }
        public static ModelMember COL_SaleInvoices_TutorTravelCost_View = new ModelMember { Name = "SaleInvoices_TutorTravelCost_View", Display = "View Tutor Travel Cost", LogDisplay = ActivityLogsController.editBooleanFormat("SaleInvoices Tutor Travel Cost View") };

        /******************************************************************************************************************************************************/

    }
}