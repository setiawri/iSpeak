using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    [Table("Payments")]
    public class PaymentsModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        [Required]
        public string No { get; set; } = string.Empty;


        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime Timestamp { get; set; } = DateTime.Now;


        [Display(Name = "Cash")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int CashAmount { get; set; } = 0;


        [Display(Name = "Debit")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int DebitAmount { get; set; } = 0;


        public string DebitBank { get; set; } = string.Empty;


        public string DebitOwnerName { get; set; } = string.Empty;


        [Display(Name = "Debit/Credit")]
        public string DebitNumber { get; set; } = string.Empty;


        public string DebitRefNo { get; set; } = string.Empty;


        public Guid? Consignments_Id { get; set; } = new Guid();


        [Display(Name = "Consignment")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int ConsignmentAmount { get; set; } = 0;


        public string Notes { get; set; } = string.Empty;


        public bool Cancelled { get; set; } = false;
        public static ModelMember COL_Cancelled = new ModelMember { Name = "Cancelled", Display = "Cancelled", LogDisplay = ActivityLogsController.editBooleanFormat("Cancelled") };


        public bool Confirmed { get; set; } = false;
        public static ModelMember COL_Confirmed = new ModelMember { Name = "Confirmed", Display = "Approval", LogDisplay = ActivityLogsController.editBooleanFormat("Approval") };


        public bool IsTransfer { get; set; } = false;


        public string CancelNotes { get; set; } = null;
        public static ModelMember COL_CancelNotes = new ModelMember { Name = "CancelNotes", Display = "Cancel Notes", LogDisplay = ActivityLogsController.editStringFormat2("Cancelled. Notes") };


        /******************************************************************************************************************************************************/

    }

}