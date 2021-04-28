using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class InventoryModel
    {
        [Key]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        public string Notes { get; set; } = string.Empty;
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Notes") };


        public Guid Branches_Id { get; set; }
        public static ModelMember COL_Branches_Id = new ModelMember { Name = "Branches_Id", Display = "Branch", LogDisplay = ActivityLogsController.editStringFormat("Branch") };


        [Display(Name = "Product")]
        public Guid Products_Id { get; set; }
        public static ModelMember COL_Products_Id = new ModelMember { Name = "Products_Id", Display = "Product", LogDisplay = ActivityLogsController.editStringFormat("Product") };
        public string Products_Name { get; set; } = string.Empty;


        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yy}")]
        public DateTime ReceiveDate { get; set; } = DateTime.Now;
        public static ModelMember COL_ReceiveDate = new ModelMember { Name = "ReceiveDate", Display = "Date", LogDisplay = ActivityLogsController.editDateFormat("Receive Date") };


        [Display(Name = "Received")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int BuyQty { get; set; } = 0;
        public static ModelMember COL_BuyQty = new ModelMember { Name = "BuyQty", Display = "Received", LogDisplay = ActivityLogsController.editIntFormat("Buy Qty") };


        [Display(Name = "Available")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int AvailableQty { get; set; } = 0;
        public static ModelMember COL_AvailableQty = new ModelMember { Name = "AvailableQty", Display = "Available", LogDisplay = ActivityLogsController.editIntFormat("Available Qty") };


        [Display(Name = "Supplier")]
        public Guid Suppliers_Id { get; set; }
        public static ModelMember COL_Suppliers_Id = new ModelMember { Name = "Suppliers_Id", Display = "Supplier", LogDisplay = ActivityLogsController.editStringFormat("Supplier") };
        public string Suppliers_Name { get; set; } = string.Empty;


        [Display(Name = "Price")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int BuyPrice { get; set; } = 0;
        public static ModelMember COL_BuyPrice = new ModelMember { Name = "BuyPrice", Display = "Price", LogDisplay = ActivityLogsController.editIntFormat("Buy Price") };


        /******************************************************************************************************************************************************/

        [Display(Name = "Global")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int GlobalAvailableQty { get; set; } = 0;

        public long InitialRowNumber { get; set; } = 0;
    }
}