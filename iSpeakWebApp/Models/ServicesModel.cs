using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class ServicesModel
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
        [Display(Name = "Price")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int SellPrice { get; set; } = 0;
        public static ModelMember COL_SellPrice = new ModelMember { Name = "SellPrice", Display = "Price", LogDisplay = ActivityLogsController.editIntFormat("Price") };


        /******************************************************************************************************************************************************/

        public string DDLDescription { get; set; } = "";
        public static ModelMember COL_DDLDescription = new ModelMember { Name = "DDLDescription" };
    }
}