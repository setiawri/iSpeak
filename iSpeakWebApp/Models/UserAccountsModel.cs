using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class UserAccountsModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id", Display = "Id" };


        [Display(Name = "Id")]
        public string No { get; set; }
        public static ModelMember COL_No = new ModelMember { Name = "No", Display = "No", LogDisplay = ActivityLogsController.editStringFormat("No") };


        [Required]
        public string Username { get; set; } = "default";
        public static ModelMember COL_Username = new ModelMember { Name = "Username", Display = "Username", LogDisplay = ActivityLogsController.editStringFormat("Username") };


        [Required]
        public string Password { get; set; } = "default";
        public static ModelMember COL_Password = new ModelMember { Name = "Password", Display = "Password" };


        [Required]
        [Display(Name = "Name")]
        public string Fullname { get; set; }
        public static ModelMember COL_Fullname = new ModelMember { Name = "Fullname", Display = "Name", LogDisplay = ActivityLogsController.editStringFormat("Name") };


        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Birthday { get; set; } = Helper.getCurrentDateTime();
        public static ModelMember COL_Birthday = new ModelMember { Name = "Birthday", Display = "Birthday", LogDisplay = ActivityLogsController.editDateFormat("Birthday") };


        [Required]
        [Display(Name = "Default")]
        public Guid Branches_Id { get; set; } = new Guid();
        public static ModelMember COL_Branches_Id = new ModelMember { Name = "Branches_Id", Display = "Branch", LogDisplay = ActivityLogsController.editStringFormat("Branch") };


        public string Branches { get; set; } = string.Empty;
        public static ModelMember COL_Branches = new ModelMember { Name = "Branches", Display = "Branches", LogDisplay = ActivityLogsController.editListStringFormat("Branches") };
        public List<string> Branches_List { get; set; }
        public static ModelMember COL_Branches_List = new ModelMember { Name = "Branches_List" };


        public bool Active { get; set; } = true;
        public static ModelMember COL_Active = new ModelMember { Name = "Active", Display = "Active", LogDisplay = ActivityLogsController.editBooleanFormat("Active") };


        public bool ResetPassword { get; set; } = true;
        public static ModelMember COL_ResetPassword = new ModelMember { Name = "ResetPassword", Display = "Password Reset", LogDisplay = ActivityLogsController.editBooleanFormat("Password Reset") };


        public string Email { get; set; }
        public static ModelMember COL_Email = new ModelMember { Name = "Email", Display = "Email", LogDisplay = ActivityLogsController.editStringFormat("Email") };


        public string Address { get; set; }
        public static ModelMember COL_Address = new ModelMember { Name = "Address", Display = "Address", LogDisplay = ActivityLogsController.editStringFormat("Address") };


        [Display(Name = "Phone 1")]
        public string Phone1 { get; set; }
        public static ModelMember COL_Phone1 = new ModelMember { Name = "Phone1", Display = "Phone 1", LogDisplay = ActivityLogsController.editStringFormat("Phone 1") };


        [Display(Name = "Phone 2")]
        public string Phone2 { get; set; }
        public static ModelMember COL_Phone2 = new ModelMember { Name = "Phone2", Display = "Phone 2", LogDisplay = ActivityLogsController.editStringFormat("Phone 2") };


        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Notes")};


        [Display(Name = "Interest")]
        public string Interest { get; set; }
        public static ModelMember COL_Interest = new ModelMember { Name = "Interest", Display = "Interest", LogDisplay = ActivityLogsController.editListStringFormat("Interest") };
        public List<string> Interest_List { get; set; }
        public static ModelMember COL_Interest_List = new ModelMember { Name = "Interest_List" };


        [Display(Name = "Event")]
        public Guid? PromotionEvents_Id { get; set; }
        public static ModelMember COL_PromotionEvents_Id = new ModelMember { Name = "PromotionEvents_Id", Display = "Promotion Event", LogDisplay = ActivityLogsController.editStringFormat("Promotion Event") };


        /******************************************************************************************************************************************************/


        [Display(Name = "Roles")]
        public string Roles { get; set; }
        public static ModelMember COL_Roles = new ModelMember { Name = "Roles", LogDisplay = ActivityLogsController.editListStringFormat("Roles") };
        public List<string> Roles_List { get; set; }
        public static ModelMember COL_Roles_List = new ModelMember { Name = "Roles_List" };

        [Display(Name = "Active Lesson")]
        public int ActiveLessonPackages { get; set; } = 0;
    }
}