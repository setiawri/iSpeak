using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSpeakWebApp.Models
{
    [Table("PayrollPaymentItems")]
    public class PayrollPaymentItemsModel
    {
        [Key]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        public Guid? PayrollPayments_Id { get; set; }
        public DateTime? Timestamp { get; set; }
        public string Description { get; set; }
        public decimal Hour { get; set; }
        public decimal HourlyRate { get; set; }
        public int TutorTravelCost { get; set; }
        public decimal Amount { get; set; }
        public string UserAccounts_Id { get; set; }
        public Guid UserAccounts_Id_TEMP { get; set; }
        public string CancelNotes { get; set; }
        public Guid? Branches_Id { get; set; }

        /******************************************************************************************************************************************************/
    }

}