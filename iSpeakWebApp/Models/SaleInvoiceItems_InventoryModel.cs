﻿using System;
using System.ComponentModel.DataAnnotations;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class SaleInvoiceItems_InventoryModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        [Required]
        public Guid SaleInvoiceItems_Id { get; set; }
        public static ModelMember COL_SaleInvoiceItems_Id = new ModelMember { Name = "SaleInvoiceItems_Id", Display = "SaleInvoiceItems_Id", LogDisplay = ActivityLogsController.editStringFormat("Sale Invoice Item") };
        public string SaleInvoices_No { get; set; }


        [Required]
        public Guid Inventory_Id { get; set; }
        public static ModelMember COL_Inventory_Id = new ModelMember { Name = "Inventory_Id", Display = "Inventory_Id", LogDisplay = ActivityLogsController.editStringFormat("Inventory") };


        [Required]
        [Display(Name = "Qty")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Qty { get; set; } = 0;
        public static ModelMember COL_Qty = new ModelMember { Name = "Qty", Display = "Qty", LogDisplay = ActivityLogsController.editIntFormat("Qty") };


        /******************************************************************************************************************************************************/

        public int Balance { get; set; } = 0;

        public bool SaleInvoices_Cancelled { get; set; } = false;
    }
}