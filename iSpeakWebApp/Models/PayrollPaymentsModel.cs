using System;
using System.ComponentModel.DataAnnotations;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class PayrollPaymentsModel
    {
        [Key]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime Timestamp { get; set; }
        public static ModelMember COL_Timestamp = new ModelMember { Name = "Timestamp", Display = "Timestamp", LogDisplay = ActivityLogsController.editDateFormat("Timestamp") };


        [Required]
        public string No { get; set; } = string.Empty;
        public static ModelMember COL_No = new ModelMember { Name = "No", Display = "No", LogDisplay = ActivityLogsController.editStringFormat("No") };


        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal Amount { get; set; } = 0;
        public static ModelMember COL_Amount = new ModelMember { Name = "Amount", Display = "Amount", LogDisplay = ActivityLogsController.editIntFormat("Amount") };


        public string Notes { get; set; } = null;
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Notes") };


        public string UserAccounts_Id { get; set; }

        [Required]
        [Display(Name = "Employee")]
        public Guid UserAccounts_Id_TEMP { get; set; }
        public static ModelMember COL_UserAccounts_Id_TEMP = new ModelMember { Name = "UserAccounts_Id_TEMP", Display = "Employee", LogDisplay = ActivityLogsController.editStringFormat("Employee") };
        public string UserAccounts_Fullname { get; set; }


        public bool IsChecked { get; set; } = false;
        public static ModelMember COL_IsChecked = new ModelMember { Name = "IsChecked", Display = "IsChecked", LogDisplay = ActivityLogsController.editBooleanFormat("Approval") };


        public bool Cancelled { get; set; } = false;
        public static ModelMember COL_Cancelled = new ModelMember { Name = "Cancelled", Display = "Cancelled", LogDisplay = ActivityLogsController.editBooleanFormat("Cancelled") };


        public string Notes_Cancel { get; set; } = null;
        public static ModelMember COL_Notes_Cancel = new ModelMember { Name = "Notes_Cancel", Display = "Cancel Notes", LogDisplay = ActivityLogsController.editStringFormat2("Cancelled. Notes") };

        /******************************************************************************************************************************************************/
    }

}