using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    [Table("PromotionEvents")]
    public class PromotionEventsModel
    {
        [Key]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        public Guid Branches_Id { get; set; }
        public static ModelMember COL_Branches_Id = new ModelMember { Name = "Branches_Id", Display = "Branches_Id", LogDisplay = ActivityLogsController.editStringFormat("Branches_Id") };


        [Required]
        public string Name { get; set; }
        public static ModelMember COL_Name = new ModelMember { Name = "Name", Display = "Name", LogDisplay = ActivityLogsController.editStringFormat("Name") };


        [Required]
        public string Location { get; set; }
        public static ModelMember COL_Location = new ModelMember { Name = "Location", Display = "Location", LogDisplay = ActivityLogsController.editStringFormat("Location") };


        [Display(Name = "Days")]
        public int TotalDays { get; set; } = 0;
        public static ModelMember COL_TotalDays = new ModelMember { Name = "TotalDays", Display = "Days", LogDisplay = ActivityLogsController.editIntFormat("Total Days") };


        [Display(Name = "Event Fee")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int EventFee { get; set; } = 0;
        public static ModelMember COL_EventFee = new ModelMember { Name = "EventFee", Display = "Event Fee", LogDisplay = ActivityLogsController.editIntFormat("Event Fee") };


        [Display(Name = "Personnel Cost")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int PersonnelCost { get; set; } = 0;
        public static ModelMember COL_PersonnelCost = new ModelMember { Name = "PersonnelCost", Display = "Personnel Cost", LogDisplay = ActivityLogsController.editIntFormat("Personnel Cost") };


        [Display(Name = "Other Cost")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int AdditionalCost { get; set; } = 0;
        public static ModelMember COL_AdditionalCost = new ModelMember { Name = "AdditionalCost", Display = "Other Cost", LogDisplay = ActivityLogsController.editIntFormat("Other Cost") };


        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Notes") };
    }
}