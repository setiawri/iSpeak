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

    }
}