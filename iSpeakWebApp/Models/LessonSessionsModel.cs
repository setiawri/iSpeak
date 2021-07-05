using System;
using System.ComponentModel.DataAnnotations;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class LessonSessionsModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        [Display(Name = "No")]
        public string No { get; set; }
        public static ModelMember COL_No = new ModelMember { Name = "No", Display = "No", LogDisplay = ActivityLogsController.editStringFormat("No") };


        public Guid? Branches_Id { get; set; } = new Guid();
        public static ModelMember COL_Branches_Id = new ModelMember { Name = "Branches_Id" };


        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime Timestamp { get; set; }
        public static ModelMember COL_Timestamp = new ModelMember { Name = "Timestamp", Display = "Timestamp", LogDisplay = ActivityLogsController.editDateTimeFormat("Timestamp") };


        [Display(Name = "Package")]
        public Guid SaleInvoiceItems_Id { get; set; }
        public static ModelMember COL_SaleInvoiceItems_Id = new ModelMember { Name = "SaleInvoiceItems_Id" };


        [Display(Name = "Duration")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal SessionHours { get; set; }
        public static ModelMember COL_SessionHours = new ModelMember { Name = "SessionHours", Display = "Hours", LogDisplay = ActivityLogsController.editDecimalFormat("Hours") };


        public string Review { get; set; }
        public static ModelMember COL_Review = new ModelMember { Name = "Review", Display = "Review", LogDisplay = ActivityLogsController.editStringFormat("Review") };


        [Display(Name = "Internal Notes")]
        public string InternalNotes { get; set; }
        public static ModelMember COL_InternalNotes = new ModelMember { Name = "InternalNotes", Display = "Internal Notes", LogDisplay = ActivityLogsController.editStringFormat("Internal Notes") };


        public bool Deleted { get; set; } = false;
        public static ModelMember COL_Deleted = new ModelMember { Name = "Deleted", Display = "Deleted", LogDisplay = ActivityLogsController.editBooleanFormat("Deleted") };


        [Required]
        [Display(Name = "Tutor")]
        public Guid Tutor_UserAccounts_Id { get; set; } = new Guid();
        public static ModelMember COL_Tutor_UserAccounts_Id = new ModelMember { Name = "Tutor_UserAccounts_Id", Display = "Tutor", LogDisplay = ActivityLogsController.editStringFormat("Tutor") };
        public string Tutor_UserAccounts_Fullname { get; set; }
        public string Tutor_UserAccounts_No { get; set; }


        [Display(Name = "Hourly Rate")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal HourlyRates_Rate { get; set; } = 0;
        public static ModelMember COL_HourlyRates_Rate = new ModelMember { Name = "HourlyRates_Rate", Display = "Hourly Rate", LogDisplay = ActivityLogsController.editDecimalFormat("Hourly Rate") };


        [Display(Name = "Travel Cost")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TravelCost { get; set; } = 0;
        public static ModelMember COL_TravelCost = new ModelMember { Name = "TravelCost", Display = "Travel Cost", LogDisplay = ActivityLogsController.editIntFormat("Travel Cost") };


        [Display(Name = "Tutor Travel Cost")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TutorTravelCost { get; set; } = 0;
        public static ModelMember COL_TutorTravelCost = new ModelMember { Name = "TutorTravelCost", Display = "Tutor Travel Cost", LogDisplay = ActivityLogsController.editIntFormat("Tutor Travel Cost") };


        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal Adjustment { get; set; } = 0;
        public static ModelMember COL_Adjustment = new ModelMember { Name = "Adjustment", Display = "Adjustment", LogDisplay = ActivityLogsController.editDecimalFormat("Adjustment") };


        public Guid? PayrollPaymentItems_Id { get; set; } = null;
        public static ModelMember COL_PayrollPaymentItems_Id = new ModelMember { Name = "PayrollPaymentItems_Id" };


        public string Notes_Cancel { get; set; } = string.Empty;
        public static ModelMember COL_Notes_Cancel = new ModelMember { Name = "Notes_Cancel", Display = "Cancel Notes", LogDisplay = ActivityLogsController.editStringFormat("Cancelled. Notes") };


        [Display(Name = "Schedule Change")]
        public bool IsScheduleChange { get; set; } = false;
        public static ModelMember COL_IsScheduleChange = new ModelMember { Name = "IsScheduleChange", Display = "Schedule Change", LogDisplay = ActivityLogsController.editBooleanFormat("Schedule Change") };


        [Display(Name = "Waive Tutor Fee")]
        public bool IsWaiveTutorFee { get; set; } = false;
        public static ModelMember COL_IsWaiveTutorFee = new ModelMember { Name = "IsWaiveTutorFee", Display = "Waive Tutor Fee", LogDisplay = ActivityLogsController.editBooleanFormat("Waive Tutor Fee") };


        /******************************************************************************************************************************************************/

        public long InitialRowNumber { get; set; } = 0;


        [Display(Name = "Lesson Package")]
        public string SaleInvoiceItems_Description { get; set; } = string.Empty;


        public string SaleInvoices_No { get; set; } = string.Empty;


        public Guid? Student_UserAccounts_Id { get; set; } = null;
        [Display(Name = "Student")]
        public string Student_UserAccounts_Fullname { get; set; } = string.Empty;
        public string Student_UserAccounts_No { get; set; } = string.Empty;

    }

}