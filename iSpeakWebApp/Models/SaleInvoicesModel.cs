using System;
using System.ComponentModel.DataAnnotations;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class SaleInvoicesModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Notes") };


        [Required]
        [Display(Name = "Branch")]
        public Guid Branches_Id { get; set; }
        public static ModelMember COL_Branches_Id = new ModelMember { Name = "Branches_Id", Display = "Branch", LogDisplay = ActivityLogsController.editStringFormat("Branch") };
        public string Branches_Name { get; set; }


        [Required]
        public string No { get; set; }
        public static ModelMember COL_No = new ModelMember { Name = "No", Display = "No", LogDisplay = ActivityLogsController.editStringFormat("No") };


        [Required]
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yy HH:mm}")]
        public DateTime Timestamp { get; set; }
        public static ModelMember COL_Timestamp = new ModelMember { Name = "Timestamp", Display = "Date", LogDisplay = ActivityLogsController.editDateTimeFormat("Timestamp") };


        [Required]
        [Display(Name = "Customer")]
        public Guid Customer_UserAccounts_Id { get; set; }
        public static ModelMember COL_Customer_UserAccounts_Id = new ModelMember { Name = "Customer_UserAccounts_Id", Display = "Branch", LogDisplay = ActivityLogsController.editStringFormat("Branch") };
        [Display(Name = "Customer")]
        public string Customer_UserAccounts_Name { get; set; }



        [Required]
        [Display(Name = "Amount")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Amount { get; set; } = 0;
        public static ModelMember COL_Amount = new ModelMember { Name = "Amount", Display = "Amount", LogDisplay = ActivityLogsController.editIntFormat("Amount") };


        [Required]
        [Display(Name = "Due")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Due { get; set; } = 0;
        public static ModelMember COL_Due = new ModelMember { Name = "Due", Display = "Due", LogDisplay = ActivityLogsController.editIntFormat("Due") };


        [Required]
        public bool Cancelled { get; set; } = true;
        public static ModelMember COL_Cancelled = new ModelMember { Name = "Cancelled", Display = "Cancelled", LogDisplay = ActivityLogsController.editBooleanFormat("Cancelled") };


        [Required]
        public bool IsChecked { get; set; } = true;
        public static ModelMember COL_IsChecked = new ModelMember { Name = "IsChecked", Display = "IsChecked", LogDisplay = ActivityLogsController.editBooleanFormat("IsChecked") };

        /******************************************************************************************************************************************************/

    }
}