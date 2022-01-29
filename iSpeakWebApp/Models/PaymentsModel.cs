using System;
using System.ComponentModel.DataAnnotations;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class PaymentsModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        [Required]
        public string No { get; set; } = string.Empty;
        public static ModelMember COL_No = new ModelMember { Name = "No" };


        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime Timestamp { get; set; } = Helper.getCurrentDateTime();
        public static ModelMember COL_Timestamp = new ModelMember { Name = "Timestamp" };


        [Display(Name = "Cash")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int CashAmount { get; set; } = 0;
        public static ModelMember COL_CashAmount = new ModelMember { Name = "CashAmount" };


        [Display(Name = "Debit")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int DebitAmount { get; set; } = 0;
        public static ModelMember COL_DebitAmount = new ModelMember { Name = "DebitAmount" };


        public string DebitBank { get; set; } = string.Empty;
        public static ModelMember COL_DebitBank = new ModelMember { Name = "DebitBank" };


        public string DebitOwnerName { get; set; } = string.Empty;
        public static ModelMember COL_DebitOwnerName = new ModelMember { Name = "DebitOwnerName" };


        [Display(Name = "Debit/Credit")]
        public string DebitNumber { get; set; } = string.Empty;
        public static ModelMember COL_DebitNumber = new ModelMember { Name = "DebitNumber" };


        public string DebitRefNo { get; set; } = string.Empty;
        public static ModelMember COL_DebitRefNo = new ModelMember { Name = "DebitRefNo" };


        public Guid? Consignments_Id { get; set; } = new Guid();
        public static ModelMember COL_Consignments_Id = new ModelMember { Name = "Consignments_Id" };
        public string Consignments_Name { get; set; }


        [Display(Name = "Consignment")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int ConsignmentAmount { get; set; } = 0;
        public static ModelMember COL_ConsignmentAmount = new ModelMember { Name = "ConsignmentAmount" };


        public string Notes { get; set; } = string.Empty;
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes" };


        public bool Cancelled { get; set; } = false;
        public static ModelMember COL_Cancelled = new ModelMember { Name = "Cancelled", Display = "Cancelled", LogDisplay = ActivityLogsController.editBooleanFormat("Cancelled") };


        [Display(Name = "Approved")]
        public bool Confirmed { get; set; } = false;
        public static ModelMember COL_Confirmed = new ModelMember { Name = "Confirmed", Display = "Approved", LogDisplay = ActivityLogsController.editBooleanFormat("Approved") };


        public bool IsTransfer { get; set; } = false;
        public static ModelMember COL_IsTransfer = new ModelMember { Name = "IsTransfer" };


        public string CancelNotes { get; set; } = null;
        public static ModelMember COL_CancelNotes = new ModelMember { Name = "CancelNotes", Display = "Cancel Notes", LogDisplay = ActivityLogsController.editStringFormat2("Cancelled. Notes") };


        /******************************************************************************************************************************************************/

    }

}