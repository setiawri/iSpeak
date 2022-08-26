using System;
using System.ComponentModel.DataAnnotations;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class ClubClassesModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        [Required]
        public string Name { get; set; } = String.Empty;
        public static ModelMember COL_Name = new ModelMember { Name = "Name", Display = "Name", LogDisplay = ActivityLogsController.editStringFormat("Name") };


        [Required]
        [Display(Name = "Language")]
        public Guid Languages_Id { get; set; }
        public static ModelMember COL_Languages_Id = new ModelMember { Name = "Languages_Id", Display = "Language", LogDisplay = ActivityLogsController.editStringFormat("Language") };
        public string Languages_Name { get; set; }


        [Display(Name = "Active")]
        public bool Active { get; set; } = true;
        public static ModelMember COL_Active = new ModelMember { Name = "Active", Display = "Active", LogDisplay = ActivityLogsController.editBooleanFormat("Active") };


        public string Notes { get; set; } = null;
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Notes") };


        [Display(Name = "Period Start")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}")]
        public DateTime? PeriodStartDate { get; set; }
        public static ModelMember COL_PeriodStartDate = new ModelMember { Name = "PeriodStartDate", Display = "PeriodStartDate", LogDisplay = ActivityLogsController.editDateTimeFormat("PeriodStartDate") };


        [Display(Name = "Adjustment Days")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int PeriodAdjustmentDayCount { get; set; } = 0;
        public static ModelMember COL_PeriodAdjustmentDayCount = new ModelMember { Name = "PeriodAdjustmentDayCount", Display = "Adjustment Days", LogDisplay = ActivityLogsController.editIntFormat("Period Adjustment Day Count") };

        /******************************************************************************************************************************************************/

    }
}