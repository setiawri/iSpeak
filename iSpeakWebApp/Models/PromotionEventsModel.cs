using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iSpeakWebApp.Models
{
    [Table("PromotionEvents")]
    public class PromotionEventsModel
    {
        [Key]
        public Guid Id { get; set; }
        public static ModelMember COL_Id = new ModelMember { Name = "Id", Display = "Id", LogDisplay = "Id" };


        public Guid Branches_Id { get; set; }
        public static ModelMember COL_Branches_Id = new ModelMember { Name = "Branches_Id", Display = "Branches_Id", LogDisplay = "Branches_Id" };


        [Required]
        public string Name { get; set; }
        public static ModelMember COL_Name = new ModelMember { Name = "Name", Display = "Name", LogDisplay = "Name" };


        [Required]
        public string Location { get; set; }
        public static ModelMember COL_Location = new ModelMember { Name = "Location", Display = "Location", LogDisplay = "Location" };


        [Display(Name = "Days")]
        public int TotalDays { get; set; } = 0;
        public static ModelMember COL_TotalDays = new ModelMember { Name = "TotalDays", Display = "Days", LogDisplay = "Total Days" };


        [Display(Name = "Event Fee")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int EventFee { get; set; } = 0;
        public static ModelMember COL_EventFee = new ModelMember { Name = "EventFee", Display = "Event Fee", LogDisplay = "Event Fee" };


        [Display(Name = "Personnel Cost")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int PersonnelCost { get; set; } = 0;
        public static ModelMember COL_PersonnelCost = new ModelMember { Name = "PersonnelCost", Display = "Personnel Cost", LogDisplay = "Personnel Cost" };


        [Display(Name = "Other Cost")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int AdditionalCost { get; set; } = 0;
        public static ModelMember COL_AdditionalCost = new ModelMember { Name = "AdditionalCost", Display = "Other Cost", LogDisplay = "Other Cost" };


        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = "Notes" };
    }
}