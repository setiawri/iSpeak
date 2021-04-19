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


        public Guid Reference_Id { get; set; }


        public int Amount { get; set; }


        public int DueBefore { get; set; }


        public int DueAfter { get; set; }


        /******************************************************************************************************************************************************/

        public string SaleInvoices_No { get; set; }

    }
}