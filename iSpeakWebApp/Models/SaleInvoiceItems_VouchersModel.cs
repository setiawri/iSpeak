using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    [Table("SlaeInvoiceItems_Vouchers")]
    public class SlaeInvoiceItems_VouchersModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        [Display(Name = "Vouchers")]
        public string Voucher_Ids { get; set; }
        public static ModelMember COL_Voucher_Ids = new ModelMember { Name = "Voucher_Ids", LogDisplay = ActivityLogsController.editListStringFormat("Vouchers") };
        public List<string> Voucher_Ids_List { get; set; }
        public static ModelMember COL_Voucher_Ids_List = new ModelMember { Name = "Vouchers_List" };


        [Required]
        [Display(Name = "Amount")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Amount { get; set; } = 0;
        public static ModelMember COL_Amount = new ModelMember { Name = "Amount", Display = "Amount", LogDisplay = ActivityLogsController.editIntFormat("Amount") };


        /******************************************************************************************************************************************************/

    }
}