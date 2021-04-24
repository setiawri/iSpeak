using System;
using System.ComponentModel.DataAnnotations;

namespace iSpeakWebApp.Models
{
    public class PaymentItemsModel
    {
        [Key]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        public Guid Payments_Id { get; set; }
        public static ModelMember COL_Payments_Id = new ModelMember { Name = "Payments_Id" };
        public string Payments_No { get; set; }


        public Guid ReferenceId { get; set; }
        public static ModelMember COL_ReferenceId = new ModelMember { Name = "ReferenceId" };


        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Amount { get; set; }
        public static ModelMember COL_Amount = new ModelMember { Name = "Amount" };


        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int DueBefore { get; set; }
        public static ModelMember COL_DueBefore = new ModelMember { Name = "DueBefore" };


        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int DueAfter { get; set; }
        public static ModelMember COL_DueAfter = new ModelMember { Name = "DueAfter" };


        /******************************************************************************************************************************************************/

        public string SaleInvoices_No { get; set; }


        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int SaleInvoices_Amount { get; set; }

    }
}