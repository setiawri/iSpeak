using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    [Table("LessonSessions")]
    public class LessonSessionsModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        public Guid? Branches_Id { get; set; }
        public static ModelMember COL_Branches_Id = new ModelMember { Name = "Branches_Id" };


        public DateTime Timestamp { get; set; }
        public static ModelMember COL_Timestamp = new ModelMember { Name = "Timestamp", Display = "Timestamp", LogDisplay = ActivityLogsController.editDateTimeFormat("Timestamp") };


        public Guid SaleInvoiceItems_Id { get; set; }
        public static ModelMember COL_SaleInvoiceItems_Id = new ModelMember { Name = "SaleInvoiceItems_Id" };


        [Display(Name = "Hours")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal SessionHours { get; set; }
        public static ModelMember COL_ = new ModelMember { Name = "SessionHours", Display = "Hours", LogDisplay = ActivityLogsController.editDecimalFormat("Hours") };


        public string Review { get; set; }
        public static ModelMember COL_Review = new ModelMember { Name = "Review", Display = "Review", LogDisplay = ActivityLogsController.editStringFormat("Review") };


        [Display(Name = "Internal Notes")]
        public string InternalNotes { get; set; }
        public static ModelMember COL_InternalNotes = new ModelMember { Name = "InternalNotes", Display = "Internal Notes", LogDisplay = ActivityLogsController.editStringFormat("Internal Notes") };


        public bool Deleted { get; set; }
        public static ModelMember COL_Deleted = new ModelMember { Name = "Deleted", Display = "Deleted", LogDisplay = ActivityLogsController.editBooleanFormat("Deleted") };


        [Required]
        [Display(Name = "Tutor")]
        public string Tutor_UserAccounts_Id { get; set; }
        public static ModelMember COL_Tutor_UserAccounts_Id = new ModelMember { Name = "Tutor_UserAccounts_Id", Display = "Tutor", LogDisplay = ActivityLogsController.editStringFormat("Tutor") };


        [Display(Name = "Hourly Rate")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal HourlyRates_Rate { get; set; }
        public static ModelMember COL_HourlyRates_Rate = new ModelMember { Name = "HourlyRates_Rate", Display = "Hourly Rate", LogDisplay = ActivityLogsController.editDecimalFormat("Hourly Rate") };


        [Display(Name = "Travel Cost")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TravelCost { get; set; }
        public static ModelMember COL_TravelCost = new ModelMember { Name = "TravelCost", Display = "Travel Cost", LogDisplay = ActivityLogsController.editIntFormat("Travel Cost") };


        [Display(Name = "Tutor Travel Cost")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int TutorTravelCost { get; set; }
        public static ModelMember COL_TutorTravelCost = new ModelMember { Name = "TutorTravelCost", Display = "Tutor Travel Cost", LogDisplay = ActivityLogsController.editIntFormat("Tutor Travel Cost") };


        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal Adjustment { get; set; }
        public static ModelMember COL_Adjustment = new ModelMember { Name = "Adjustment", Display = "Adjustment", LogDisplay = ActivityLogsController.editDecimalFormat("Adjustment") };


        public Guid? PayrollPaymentItems_Id { get; set; }
        public static ModelMember COL_PayrollPaymentItems_Id = new ModelMember { Name = "PayrollPaymentItems_Id" };


        public string Notes_Cancel { get; set; }
        public static ModelMember COL_Notes_Cancel = new ModelMember { Name = "Notes_Cancel", Display = "Cancel Notes", LogDisplay = ActivityLogsController.editStringFormat("Cancelled. Notes") };


        public bool IsScheduleChange { get; set; }
        public static ModelMember COL_IsScheduleChange = new ModelMember { Name = "IsScheduleChange", Display = "Schedule Change", LogDisplay = ActivityLogsController.editBooleanFormat("Schedule Change") };


        public bool IsWaiveTutorFee { get; set; }
        public static ModelMember COL_IsWaiveTutorFee = new ModelMember { Name = "IsWaiveTutorFee", Display = "Waive Tutor Fee", LogDisplay = ActivityLogsController.editBooleanFormat("Waive Tutor Fee") };


        /******************************************************************************************************************************************************/

    }

}