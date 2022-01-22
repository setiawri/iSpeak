using System;
using System.ComponentModel.DataAnnotations;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class TutorStudentSchedulesModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        [Required]
        [Display(Name = "Tutor")]
        public Guid Tutor_UserAccounts_Id { get; set; }
        public static ModelMember COL_Tutor_UserAccounts_Id = new ModelMember { Name = "Tutor_UserAccounts_Id" };
        public string Tutor_UserAccounts_Name { get; set; }
        public string Tutor_UserAccounts_No { get; set; }


        [Required]
        [Display(Name = "Student")]
        public Guid Student_UserAccounts_Id { get; set; }
        public static ModelMember COL_Student_UserAccounts_Id = new ModelMember { Name = "Student_UserAccounts_Id" };
        public string Student_UserAccounts_Name { get; set; }
        public string Student_UserAccounts_No { get; set; }


        [Required]
        [Display(Name = "Day")]
        public DayOfWeekEnum DayOfWeek { get; set; }
        public static ModelMember COL_DayOfWeek = new ModelMember { Name = "DayOfWeek", Display = "Day of Week", LogDisplay = ActivityLogsController.editStringFormat("Day of Week") };


        [Required]
        [Display(Name = "Start")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        public DateTime StartTime { get; set; }
        public static ModelMember COL_StartTime = new ModelMember { Name = "StartTime", Display = "Start Time", LogDisplay = ActivityLogsController.editTimeFormat("Start Time") };


        [Required]
        [Display(Name = "End")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        public DateTime EndTime { get; set; }
        public static ModelMember COL_EndTime = new ModelMember { Name = "EndTime", Display = "End Time", LogDisplay = ActivityLogsController.editTimeFormat("End Time") };


        [Display(Name = "Invoice")]
        public Guid SaleInvoiceItems_Id { get; set; } = Guid.Empty;
        public static ModelMember COL_SaleInvoiceItems_Id = new ModelMember { Name = "SaleInvoiceItems_Id" };
        public string SaleInvoiceItems_Description { get; set; }
        public string SaleInvoices_No { get; set; }
        public string Languages_Name { get; set; }
        public decimal SessionHours_Remaining { get; set; }


        [Display(Name = "Active")]
        public bool Active { get; set; } = true;
        public static ModelMember COL_Active = new ModelMember { Name = "Active", Display = "Active", LogDisplay = ActivityLogsController.editBooleanFormat("Active") };


        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Notes") };


        /******************************************************************************************************************************************************/

    }
}