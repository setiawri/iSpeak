using System;
using System.ComponentModel.DataAnnotations;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class ProductsModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        [Required]
        public string Name { get; set; }
        public static ModelMember COL_Name = new ModelMember { Name = "Name", Display = "Name", LogDisplay = ActivityLogsController.editStringFormat("Name") };


        [Required]
        public bool Active { get; set; } = true;
        public static ModelMember COL_Active = new ModelMember { Name = "Active", Display = "Active", LogDisplay = ActivityLogsController.editBooleanFormat("Active") };


        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Notes") };


        public string Description { get; set; }
        public static ModelMember COL_Description = new ModelMember { Name = "Description", Display = "Description", LogDisplay = ActivityLogsController.editStringFormat("Description") };


        public string Barcode { get; set; }
        public static ModelMember COL_Barcode = new ModelMember { Name = "Barcode", Display = "Barcode", LogDisplay = ActivityLogsController.editStringFormat("Barcode") };


        [Required]
        [Display(Name = "Unit")]
        public Guid Units_Id { get; set; }
        public static ModelMember COL_Units_Id = new ModelMember { Name = "Units_Id", Display = "Unit", LogDisplay = ActivityLogsController.editStringFormat("Unit") };
        public string Units_Name { get; set; }


        [Required]
        [Display(Name = "For Sale")]
        public bool ForSale { get; set; } = true;
        public static ModelMember COL_ForSale = new ModelMember { Name = "ForSale", Display = "For Sale", LogDisplay = ActivityLogsController.editBooleanFormat("For Sale") };


        [Required]
        [Display(Name = "Buy Price")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int BuyPrice { get; set; } = 0;
        public static ModelMember COL_BuyPrice = new ModelMember { Name = "BuyPrice", Display = "Buy Price", LogDisplay = ActivityLogsController.editIntFormat("Buy Price") };


        [Required]
        [Display(Name = "Sell Price")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int SellPrice { get; set; } = 0;
        public static ModelMember COL_SellPrice = new ModelMember { Name = "SellPrice", Display = "Sell Price", LogDisplay = ActivityLogsController.editIntFormat("Sell Price") };


        [Required]
        [Display(Name = "Franchise")]
        public Guid Franchises_Id { get; set; }
        public static ModelMember COL_Franchises_Id = new ModelMember { Name = "Franchises_Id", Display = "Franchise", LogDisplay = ActivityLogsController.editStringFormat("Franchise") };
        public string Franchises_Name { get; set; } = string.Empty;

        /******************************************************************************************************************************************************/

        public string DDLDescription { get; set; } = "";
        public static ModelMember COL_DDLDescription = new ModelMember { Name = "DDLDescription" };

        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int AvailableQty { get; set; } = 0;
        public static ModelMember COL_AvailableQty = new ModelMember { Name = "AvailableQty" };
    }
}