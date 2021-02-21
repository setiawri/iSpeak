using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSpeakWebApp.Models
{
    [Table("UserAccountRoles")]
    public class UserAccountRolesModel
    {
        [Key]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id", Display = "Id", LogDisplay = "" };

        public string Name { get; set; }
        public static ModelMember COL_Name = new ModelMember { Name = "Name", Display = "Name", LogDisplay = "Name" };

        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = "Notes" };

        /* REMINDERS ******************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string Reminders_Notes { get; set; }
        public static ModelMember COL_Reminders_Notes = new ModelMember { Name = "Reminders_Notes", Display = "Notes", LogDisplay="Reminder Notes" };

        [Display(Name = "Add")]
        public bool Reminders_Add { get; set; }
        public static ModelMember COL_Reminders_Add = new ModelMember { Name = "Reminders_Add", Display = "Add", LogDisplay = "Reminder Add" };

        [Display(Name = "View")]
        public bool Reminders_View { get; set; }
        public static ModelMember COL_Reminders_View = new ModelMember { Name = "Reminders_View", Display = "View", LogDisplay = "Reminder View" };

        [Display(Name = "Edit")]
        public bool Reminders_Edit { get; set; }
        public static ModelMember COL_Reminders_Edit = new ModelMember { Name = "Reminders_Edit", Display = "Edit", LogDisplay = "Reminder Edit" };
        
        /* USER ACCOUNTS ROLES ********************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string UserAccountRoles_Notes { get; set; }
        public static ModelMember COL_UserAccountRoles_Notes = new ModelMember { Name = "UserAccountRoles_Notes", Display = "Notes", LogDisplay="Branches Notes" };

        [Display(Name = "Add")]
        public bool UserAccountRoles_Add { get; set; }
        public static ModelMember COL_UserAccountRoles_Add = new ModelMember { Name = "UserAccountRoles_Add", Display = "Add", LogDisplay = "Branches Add" };

        [Display(Name = "View")]
        public bool UserAccountRoles_View { get; set; }
        public static ModelMember COL_UserAccountRoles_View = new ModelMember { Name = "UserAccountRoles_View", Display = "View", LogDisplay = "Branches View" };

        [Display(Name = "Edit")]
        public bool UserAccountRoles_Edit { get; set; }
        public static ModelMember COL_UserAccountRoles_Edit = new ModelMember { Name = "UserAccountRoles_Edit", Display = "Edit", LogDisplay = "Branches Edit" };

        /* APPLICATION SETTINGS *******************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string Settings_Notes { get; set; }
        public static ModelMember COL_Settings_Notes = new ModelMember { Name = "Settings_Notes", Display = "Notes", LogDisplay = "Settings Roles Notes" };

        [Display(Name = "View")]
        public bool Settings_View { get; set; }
        public static ModelMember COL_Settings_View = new ModelMember { Name = "Settings_View", Display = "View", LogDisplay = "Settings Roles View" };

        [Display(Name = "Edit")]
        public bool Settings_Edit { get; set; }
        public static ModelMember COL_Settings_Edit = new ModelMember { Name = "Settings_Edit", Display = "Edit", LogDisplay = "Settings Roles Edit" };

        /* BRANCHES *******************************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string Branches_Notes { get; set; }
        public static ModelMember COL_Branches_Notes = new ModelMember { Name = "Branches_Notes", Display = "Notes", LogDisplay = "Branches Notes" };

        [Display(Name = "Add")]
        public bool Branches_Add { get; set; }
        public static ModelMember COL_Branches_Add = new ModelMember { Name = "Branches_Add", Display = "Add", LogDisplay = "Branches Add" };

        [Display(Name = "View")]
        public bool Branches_View { get; set; }
        public static ModelMember COL_Branches_View = new ModelMember { Name = "Branches_View", Display = "View", LogDisplay = "Branches View" };

        [Display(Name = "Edit")]
        public bool Branches_Edit { get; set; }
        public static ModelMember COL_Branches_Edit = new ModelMember { Name = "Branches_Edit", Display = "Edit", LogDisplay = "Branches Edit" };


        /* PROMOTION EVENTS ***********************************************************************************************************************************/

        [Display(Name = "Notes")]
        public string PromotionEvents_Notes { get; set; }
        public static ModelMember COL_PromotionEvents_Notes = new ModelMember { Name = "PromotionEvents_Notes", Display = "Notes", LogDisplay = "PromotionEvents Notes" };

        [Display(Name = "Add")]
        public bool PromotionEvents_Add { get; set; }
        public static ModelMember COL_PromotionEvents_Add = new ModelMember { Name = "PromotionEvents_Add", Display = "Add", LogDisplay = "PromotionEvents Add" };

        [Display(Name = "View")]
        public bool PromotionEvents_View { get; set; }
        public static ModelMember COL_PromotionEvents_View = new ModelMember { Name = "PromotionEvents_View", Display = "View", LogDisplay = "PromotionEvents View" };

        [Display(Name = "Edit")]
        public bool PromotionEvents_Edit { get; set; }
        public static ModelMember COL_PromotionEvents_Edit = new ModelMember { Name = "PromotionEvents_Edit", Display = "Edit", LogDisplay = "PromotionEvents Edit" };

        /******************************************************************************************************************************************************/

    }
}