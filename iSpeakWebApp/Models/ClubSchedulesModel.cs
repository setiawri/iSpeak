using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using iSpeakWebApp.Controllers;

namespace iSpeakWebApp.Models
{
    public class ClubSchedulesModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public static ModelMember COL_Id = new ModelMember { Name = "Id" };


        [Required]
        [Display(Name = "Day")]
        public DayOfWeekEnum DayOfWeek { get; set; }
        public static ModelMember COL_DayOfWeek = new ModelMember { Name = "DayOfWeek", Display = "Day of Week", LogDisplay = ActivityLogsController.editStringFormat("Day of Week") };


        [Required]
        [Display(Name = "Start")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        public DateTime StartTime { get; set; }
        public static ModelMember COL_StartTime = new ModelMember { Name = "StartTime", Display = "Start Time", LogDisplay = ActivityLogsController.editTimeFormat("Start Time") };


        [Required]
        [Display(Name = "End")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}")]
        public DateTime EndTime { get; set; }
        public static ModelMember COL_EndTime = new ModelMember { Name = "EndTime", Display = "End Time", LogDisplay = ActivityLogsController.editTimeFormat("End Time") };


        [Display(Name = "Active")]
        public bool Active { get; set; } = true;
        public static ModelMember COL_Active = new ModelMember { Name = "Active", Display = "Active", LogDisplay = ActivityLogsController.editBooleanFormat("Active") };


        public string Notes { get; set; }
        public static ModelMember COL_Notes = new ModelMember { Name = "Notes", Display = "Notes", LogDisplay = ActivityLogsController.editStringFormat("Notes") };


        [Display(Name = "Branch")]
        public Guid Branches_Id { get; set; }
        public static ModelMember COL_Branches_Id = new ModelMember { Name = "Branches_Id", Display = "Branch", LogDisplay = ActivityLogsController.editStringFormat("Branch") };
        public string Branches_Name { get; set; }


        [Display(Name = "On Site")]
        public bool OnSite { get; set; } = false;
        public static ModelMember COL_OnSite = new ModelMember { Name = "OnSite", Display = "On Site", LogDisplay = ActivityLogsController.editBooleanFormat("On Site") };


        [Display(Name = "Class")]
        public Guid ClubClasses_Id { get; set; }
        public static ModelMember COL_ClubClasses_Id = new ModelMember { Name = "ClubClasses_Id", Display = "Class", LogDisplay = ActivityLogsController.editBooleanFormat("ClubClasses_Id") };
        public string ClubClasses_Name { get; set; }


        /******************************************************************************************************************************************************/


        [Display(Name = "Language")]
        public string Languages_Name { get; set; }


        [Display(Name = "Language")]
        public List<ClubClassOnlineLinksModel> ClubClassOnlineLinks { get; set; } = new List<ClubClassOnlineLinksModel>();
    }
}